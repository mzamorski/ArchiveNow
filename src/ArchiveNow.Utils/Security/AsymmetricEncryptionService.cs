using System;
using System.IO;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;

namespace ArchiveNow.Utils.Security
{
    public class EncryptionOptions
    {
        public bool Armor { get; set; } = false;

        public bool WithIntegrityCheck { get; set; } = true;

        public string OutputFileExtension { get; set; } = ".pgp";

        public bool ReplaceOriginalExtension { get; set; } = true;
    }

    public class AsymmetricEncryptionService : IEncryptionService
    {
        private readonly EncryptionOptions _options = new EncryptionOptions();

        public string EncryptFile(string inputFilePath, string publicKeyFilePath)
        {
            if (!File.Exists(inputFilePath))
                throw new FileNotFoundException("Input file does not exist.", inputFilePath);

            if (!File.Exists(publicKeyFilePath))
                throw new FileNotFoundException("Public key file does not exist.", publicKeyFilePath);

            try
            {
                string outputFilePath = GetOutputFilePath(inputFilePath);

                PgpPublicKey publicKey = ReadPublicKey(publicKeyFilePath);
                byte[] compressedData = CompressFile(inputFilePath);
                EncryptData(compressedData, outputFilePath, publicKey);

                return outputFilePath;
            }
            catch (PgpException ex)
            {
                throw new InvalidOperationException("PGP encryption failed.", ex);
            }
        }

        private static byte[] CompressFile(string inputFilePath)
        {
            using (var dataStream = new MemoryStream())
            {
                var compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Uncompressed);

                using (var compressedOut = compressedDataGenerator.Open(dataStream))
                {
                    PgpUtilities.WriteFileToLiteralData(compressedOut, PgpLiteralData.Binary, new FileInfo(inputFilePath));
                }

                compressedDataGenerator.Close();
                return dataStream.ToArray();
            }
        }

        private void EncryptData(byte[] data, string outputFilePath, PgpPublicKey publicKey)
        {
            var encryptedDataGenerator = new PgpEncryptedDataGenerator(
                SymmetricKeyAlgorithmTag.Cast5,
                _options.WithIntegrityCheck,
                new SecureRandom()
            );

            encryptedDataGenerator.AddMethod(publicKey);

            using (var outputStream = File.Create(outputFilePath))
            {
                Stream finalStream = outputStream;

                if (_options.Armor)
                {
                    finalStream = new ArmoredOutputStream(outputStream);
                }

                using (finalStream)
                using (var encryptedOut = encryptedDataGenerator.Open(finalStream, data.Length))
                {
                    encryptedOut.Write(data, 0, data.Length);
                }
            }
        }

        private static PgpPublicKey ReadPublicKey(string publicKeyFilePath)
        {
            using (var keyIn = File.OpenRead(publicKeyFilePath))
            {
                return ReadPublicKey(keyIn);
            }
        }

        private static PgpPublicKey ReadPublicKey(Stream inputStream)
        {
            var pgpPub = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(inputStream));

            foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey key in keyRing.GetPublicKeys())
                {
                    if (key.IsEncryptionKey)
                        return key;
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        private string GetOutputFilePath(string inputFilePath)
        {
            var directory = Path.GetDirectoryName(inputFilePath);
            if (directory == null)
                throw new InvalidOperationException("Could not determine directory of the input file.");

            if (_options.ReplaceOriginalExtension)
            {
                var fileNameWithoutExt = Path.GetFileNameWithoutExtension(inputFilePath);
                return Path.Combine(directory, fileNameWithoutExt + _options.OutputFileExtension);
            }
            else
            {
                return Path.Combine(directory, Path.GetFileName(inputFilePath) + _options.OutputFileExtension);
            }
        }
    }
}
