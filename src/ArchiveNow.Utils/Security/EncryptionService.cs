using System;
using System.IO;

using Org.BouncyCastle.Bcpg;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Security;

namespace ArchiveNow.Utils.Security
{
    public class EncryptionService : IEncryptionService
    {
        public string EncryptFile(string inputFilePath, string publicKeyFilePath)
        {
            var encryptedFilePath = inputFilePath;

            try
            {
                EncryptFile(inputFilePath, encryptedFilePath, publicKeyFilePath, false, true);
            }
            catch (PgpException e)
            {
                throw new InvalidOperationException(e.Message);
            }

            return encryptedFilePath;
        }

        private static PgpPublicKey ReadPublicKey(string keyFilePath)
        {
            using (Stream keyFileStream = File.OpenRead(keyFilePath))
            {
                return ReadPublicKey(keyFileStream);
            }
        }

        private static PgpPublicKey ReadPublicKey(Stream keyFileStream)
        {
            var pgpPub = new PgpPublicKeyRingBundle(PgpUtilities.GetDecoderStream(keyFileStream));
            
            foreach (PgpPublicKeyRing keyRing in pgpPub.GetKeyRings())
            {
                foreach (PgpPublicKey key in keyRing.GetPublicKeys())
                {
                    if (key.IsEncryptionKey)
                    {
                        return key;
                    }
                }
            }

            throw new ArgumentException("Can't find encryption key in key ring.");
        }

        public static void EncryptFile(string inputFile, string outputFile, string publicKeyFile,
            bool armor,
            bool withIntegrityCheck
        )
        {
            PgpPublicKey publicKey = ReadPublicKey(publicKeyFile);

            using (var dataStream = new MemoryStream())
            {
                var compressedDataGenerator = new PgpCompressedDataGenerator(CompressionAlgorithmTag.Uncompressed);

                PgpUtilities.WriteFileToLiteralData(compressedDataGenerator.Open(dataStream), PgpLiteralData.Binary,
                    new FileInfo(inputFile));

                compressedDataGenerator.Close();

                var encryptedDataGenerator = new PgpEncryptedDataGenerator(SymmetricKeyAlgorithmTag.Cast5,
                    withIntegrityCheck, new SecureRandom());

                encryptedDataGenerator.AddMethod(publicKey);
                byte[] bytes = dataStream.ToArray();

                using (Stream outputStream = File.Create(outputFile))
                {
                    if (armor)
                    {
                        using (var armoredStream = new ArmoredOutputStream(outputStream))
                        {
                            Write(encryptedDataGenerator, armoredStream, bytes);
                        }
                    }
                    else
                    {
                        Write(encryptedDataGenerator, outputStream, bytes);
                    }
                }

            }
        }

        private static void Write(PgpEncryptedDataGenerator generator, Stream dataStream, byte[] bytes)
        {
            using (Stream outputStream = generator.Open(dataStream, bytes.Length))
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
    }
}
