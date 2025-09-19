using System;
using System.Net;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;

namespace ArchiveNow.Security
{
    /// <summary>
    /// Symmetric, authenticated token carrying a (potentially padded) UTF-8 filename.
    /// Token format: base64url( nonce[12] | ciphertext[N] | tag[16] ).
    /// Intended to be placed in Authorization: Bearer AN.v1.<token> or in Cookie: an_sess=<token>.
    /// </summary>
    public static class FileNameToken
    {
        public const string DefaultSchemePrefix = "AN";     // product prefix
        public const string DefaultVersion = "v1";          // for future rotation

        /// <summary>
        /// Options for token creation/parsing.
        /// </summary>
        public sealed class Options
        {
            /// <summary>256-bit key as bytes (takes precedence over KeyHex if provided).</summary>
            public byte[]? KeyBytes { get; init; }
            /// <summary>256-bit key as 64 hex chars (used if KeyBytes is null). If null, will read from env ARCHIVENOW_FN_KEY_HEX.</summary>
            public string? KeyHex { get; init; }
            /// <summary>Optional fixed pad length (bytes). If <= 0, no padding is applied.</summary>
            public int PadToBytes { get; init; } = 0; // e.g. 128 to conceal length
            /// <summary>Product prefix used in Authorization header (e.g., AN).</summary>
            public string SchemePrefix { get; init; } = DefaultSchemePrefix;
            /// <summary>Version string (e.g., v1). Placed after prefix in Authorization header.</summary>
            public string Version { get; init; } = DefaultVersion;
        }

        /// <summary>
        /// Build an AuthenticationHeaderValue like: Authorization: Bearer AN.v1.<token>.
        /// </summary>
        public static AuthenticationHeaderValue BuildAuthHeader(string fileName, Options? options = null)
        {
            options ??= new Options();
            var token = Encrypt(fileName, options);
            var param = options.SchemePrefix + "." + options.Version + "." + token;
            return new AuthenticationHeaderValue("Bearer", param);
        }

        /// <summary>
        /// Encrypt filename to base64url token using AES-GCM-256.
        /// </summary>
        public static string Encrypt(string fileName, Options? options = null)
        {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            options ??= new Options();

            var key = ResolveKey(options);
            var plain = Encoding.UTF8.GetBytes(fileName);

            // Optional zero padding to a fixed size
            if (options.PadToBytes > 0 && plain.Length < options.PadToBytes)
            {
                var padded = new byte[options.PadToBytes];
                Buffer.BlockCopy(plain, 0, padded, 0, plain.Length);
                plain = padded; // zero padded
            }

            Span<byte> nonce = stackalloc byte[12];
            RandomNumberGenerator.Fill(nonce);

            var cipher = new byte[plain.Length];
            Span<byte> tag = stackalloc byte[16];

            using (var aes = new AesGcm(key))
            {
                aes.Encrypt(nonce, plain, cipher, tag);
            }

            var blob = new byte[nonce.Length + cipher.Length + tag.Length];
            nonce.CopyTo(blob);
            Buffer.BlockCopy(cipher, 0, blob, nonce.Length, cipher.Length);
            tag.CopyTo(blob.AsSpan(nonce.Length + cipher.Length));

            return ToBase64Url(blob);
        }

        /// <summary>
        /// Decrypt base64url token to UTF-8 filename.
        /// If padding was used, trailing '\0' bytes are trimmed.
        /// Returns null on any failure.
        /// </summary>
        public static string? Decrypt(string tokenBase64Url, Options? options = null)
        {
            options ??= new Options();
            var key = ResolveKey(options);

            if (!TryFromBase64Url(tokenBase64Url, out var blob) || blob.Length < 12 + 16 + 1)
                return null;

            var nonce = blob.AsSpan(0, 12);
            var tag = blob.AsSpan(blob.Length - 16, 16);
            var cipher = blob.AsSpan(12, blob.Length - 12 - 16);

            var plain = new byte[cipher.Length];
            try
            {
                using var aes = new AesGcm(key);
                aes.Decrypt(nonce, cipher, tag, plain);
            }
            catch
            {
                return null;
            }

            // Trim padding zeros if present
            int len = plain.Length;
            while (len > 0 && plain[len - 1] == 0) len--;
            return Encoding.UTF8.GetString(plain, 0, len);
        }

        /// <summary>
        /// Server helper: try to extract and decrypt the filename from Authorization or Cookie headers.
        /// Supported: Authorization: Bearer AN.v1.<token> and Cookie: an_sess=<token>.
        /// </summary>
        public static bool TryExtractFromRequest(HttpListenerRequest req, out string fileName, Options? options = null)
        {
            options ??= new Options();

            // Authorization header
            var auth = req.Headers["Authorization"];
            if (!string.IsNullOrWhiteSpace(auth))
            {
                const string prefix = "Bearer ";
                if (auth.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    var param = auth[prefix.Length..].Trim(); // e.g. AN.v1.token
                    var parts = param.Split('.', 3, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 3 && parts[0] == options.SchemePrefix && parts[1] == options.Version)
                    {
                        var name = Decrypt(parts[2], options);
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            fileName = name;
                            return true;
                        }
                    }
                    // Back-compat: AN.<token>
                    if (parts.Length == 2 && parts[0] == options.SchemePrefix)
                    {
                        var name = Decrypt(parts[1], options);
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            fileName = name;
                            return true;
                        }
                    }
                }
            }

            // Cookie fallback
            var cookie = req.Headers["Cookie"];
            if (!string.IsNullOrEmpty(cookie))
            {
                foreach (var part in cookie.Split(';'))
                {
                    var kv = part.Split('=', 2);
                    if (kv.Length == 2 && kv[0].Trim() == "an_sess")
                    {
                        var raw = Uri.UnescapeDataString(kv[1].Trim());
                        var name = Decrypt(raw, options);
                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            fileName = name;
                            return true;
                        }
                        break;
                    }
                }
            }

            fileName = string.Empty;
            return false;
        }

        /// <summary>
        /// Client helper: set Authorization header on HttpClient.
        /// </summary>
        public static void SetAuthHeader(HttpClient client, string fileName, Options? options = null)
        {
            client.DefaultRequestHeaders.Authorization = BuildAuthHeader(fileName, options);
        }

        // -------- internals --------

        private static byte[] ResolveKey(Options options)
        {
            if (options.KeyBytes is { Length: 32 }) return options.KeyBytes;
            var hex = options.KeyHex ?? Environment.GetEnvironmentVariable("ARCHIVENOW_FN_KEY_HEX");
            if (string.IsNullOrWhiteSpace(hex) || hex.Length != 64)
                throw new InvalidOperationException("Missing 256-bit key (ARCHIVENOW_FN_KEY_HEX or Options.KeyHex; 64 hex chars).");
            return Convert.FromHexString(hex);
        }

        private static string ToBase64Url(byte[] data)
        {
            return Convert.ToBase64String(data).Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        private static bool TryFromBase64Url(string input, out byte[] data)
        {
            string s = input.Replace('-', '+').Replace('_', '/');
            switch (s.Length % 4)
            {
                case 2: s += "=="; break;
                case 3: s += "="; break;
            }
            try { data = Convert.FromBase64String(s); return true; }
            catch { data = Array.Empty<byte>(); return false; }
        }
    }
}
