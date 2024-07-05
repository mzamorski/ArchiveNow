using System.Security;

namespace ArchiveNow.Providers.Core.PasswordProviders
{
    public interface IPasswordProvider
    {
        SecureString Password { get; }
    }
}
