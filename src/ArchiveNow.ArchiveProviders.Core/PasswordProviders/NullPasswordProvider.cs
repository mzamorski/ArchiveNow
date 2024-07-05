using System;
using System.Security;

namespace ArchiveNow.Providers.Core.PasswordProviders
{
    public class NullPasswordProvider : IPasswordProvider
    {
        private static readonly Lazy<NullPasswordProvider> _instance =
            new Lazy<NullPasswordProvider>(() => new NullPasswordProvider());

        public static NullPasswordProvider Instance => _instance.Value;

        public SecureString Password => new SecureString();
    }
}