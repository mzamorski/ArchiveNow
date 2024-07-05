using ArchiveNow.Utils;

namespace ArchiveNow.Service.ArchiveProviders
{
    public static class PasswordService
    {
        public static string Encrypt(string password)
        {
            return password.Protect();
        }

        public static string Decrypt(string encryptedPassword)
        {
            return encryptedPassword.Unprotect();
        }
    }
}