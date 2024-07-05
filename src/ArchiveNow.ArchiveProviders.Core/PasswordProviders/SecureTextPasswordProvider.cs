using System.Security;

using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core.PasswordProviders
{
    public class SecureTextPasswordProvider : IPasswordProvider
    {
        public SecureString Password { get; }

        public SecureTextPasswordProvider(string encryptedPassword)
        {
            string password = encryptedPassword.Unprotect();

            Password = password.ConvertToSecureString();
            Password.MakeReadOnly();
        }
    }
}