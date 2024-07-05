using System.Security;

using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core.PasswordProviders
{
    public class PlainTextPasswordProvider : IPasswordProvider
    {
        public SecureString Password { get; }

        public PlainTextPasswordProvider(string password)
        {
            Password = password.ConvertToSecureString();
            Password.MakeReadOnly();
        }
    }
}
