using System;
using System.Security.Cryptography;
using System.Text;

namespace ArchiveNow.Utils.Security
{
    public static class PasswordHelper
    {
        private const string Letters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Digits = "0123456789";
        private const string Symbols = "!@#$%^&*()_-+=[]{}:;.,?";

        /// <summary>
        /// Rough equivalent of Membership.GeneratePassword(len, nonAlnum)
        /// </summary>
        public static string GeneratePassword(int length, int numberOfNonAlphanumericCharacters)
        {
            if (length < 1) throw new ArgumentOutOfRangeException(nameof(length));
            if (numberOfNonAlphanumericCharacters < 0 || numberOfNonAlphanumericCharacters > length)
                throw new ArgumentOutOfRangeException(nameof(numberOfNonAlphanumericCharacters));

            var all = (Letters + Digits + Symbols).ToCharArray();
            var sb = new StringBuilder(length);

            // najpierw wrzuć wymagane symbole
            for (int i = 0; i < numberOfNonAlphanumericCharacters; i++)
                sb.Append(GetRandomChar(Symbols));

            // reszta: litery/cyfry/symbole (jak chcesz – tu: litery+cyfry)
            var pool = (Letters + Digits).ToCharArray();
            for (int i = sb.Length; i < length; i++)
                sb.Append(GetRandomChar(pool));

            // potasuj, żeby symbole nie były z przodu
            return Shuffle(sb.ToString());
        }

        private static char GetRandomChar(string chars)
        {
            return GetRandomChar(chars.ToCharArray());
        }

        private static char GetRandomChar(char[] chars)
        {
            var idx = RandomNumberGenerator.GetInt32(chars.Length);
            return chars[idx];
        }

        private static string Shuffle(string input)
        {
            var arr = input.ToCharArray();
            for (int i = arr.Length - 1; i > 0; i--)
            {
                int j = RandomNumberGenerator.GetInt32(i + 1);
                (arr[i], arr[j]) = (arr[j], arr[i]);
            }
            return new string(arr);
        }
    }
}
