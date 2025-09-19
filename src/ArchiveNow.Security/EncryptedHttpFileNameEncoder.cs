using System.Net;

namespace ArchiveNow.Security
{
    public sealed class EncryptedHttpFileNameEncoder : IHttpFileNameEncoder
    {
        private readonly FileNameToken.Options _options;

        public EncryptedHttpFileNameEncoder(FileNameToken.Options? options = null)
        {
            _options = options ?? new FileNameToken.Options();
        }

        public string Decode(HttpListenerRequest req)
        {
            if (FileNameToken.TryExtractFromRequest(req, out var fileName, _options))
                return fileName;

            var fallback = $"upload_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}";
            return fallback;
        }

        public void Encode(HttpClient client, HttpContent content, string fileName)
        {
            FileNameToken.SetAuthHeader(client, fileName, _options);
        }
    }
}
