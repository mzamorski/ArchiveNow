namespace ArchiveNow.Utils.Security
{
    public interface IEncryptionService
    {
        string EncryptFile(string inputFilePath, string publicKeyFilePath);
    }
}