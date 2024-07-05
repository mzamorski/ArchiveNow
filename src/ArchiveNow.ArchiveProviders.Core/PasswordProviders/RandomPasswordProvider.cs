using System.Security;
using System.Web.Security;

using ArchiveNow.Utils;

namespace ArchiveNow.Providers.Core.PasswordProviders
{
    public class RandomPasswordProvider : IPasswordProvider
    {
        private const int DefaultLength = 16;

        public int Length { get; }

        public SecureString Password { get; }

        public RandomPasswordProvider(int? length = null)
        {
            if ((length == null) || (length.Value == 0))
            {
                this.Length = DefaultLength;
            }
            else
            {
                this.Length = length.Value;
            }

            var password = Membership.GeneratePassword(this.Length, 0);

            this.Password = password.ConvertToSecureString();
            this.Password.MakeReadOnly();
        }
    }
}