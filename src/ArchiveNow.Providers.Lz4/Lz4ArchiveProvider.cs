using System.IO;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.PasswordProviders;

using K4os.Compression.LZ4.Streams;

namespace ArchiveNow.Providers.Lz4
{
    public class Lz4ArchiveProvider : ArchiveProviderBase
    {
        private readonly Stream _archiveStream;
        private LZ4EncoderStream _lz4Stream;

        public override string FileExtension => "lz4";

        public Lz4ArchiveProvider(IArchiveFilePathBuilder archiveFilePathBuilder, IPasswordProvider passwordProvider)
            : base(archiveFilePathBuilder, passwordProvider)
        {
            _archiveStream = File.Create(ArchiveFilePath);
        }

        public override void AddDirectory(string path)
        {
            CompressDirectory(path);
        }

        public override void Add(string path)
        {
            CompressFile(path);

            OnFileCompressed();
        }

        public override void BeginUpdate(string sourcePath)
        {
            // Create LZ4 compression stream
            _lz4Stream = LZ4Stream.Encode(_archiveStream, leaveOpen: true);
        }

        public override void CommitUpdate()
        {
            CurrentProgressMode = ProgressMode.Indeterminate;

            OnFinished();
        }

        public override void AbortUpdate()
        { }

        private void CompressDirectory(string directoryPath)
        { }

        private void CompressFile(string filePath)
        {
            using (var fileStream = File.OpenRead(filePath))
            {
                fileStream.CopyTo(_lz4Stream);
            }
        }
    }
}
