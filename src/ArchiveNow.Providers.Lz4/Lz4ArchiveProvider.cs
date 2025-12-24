using System.IO;
using System.Text;

using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Providers.Core.EntryTransforms;

using K4os.Compression.LZ4;
using K4os.Compression.LZ4.Streams;
using ICSharpCode.SharpZipLib.Tar;

namespace ArchiveNow.Providers.Lz4
{
    public class Lz4ArchiveProvider : ArchiveProviderBase
    {
        private Stream _fileStream;
        private LZ4EncoderStream _lz4Stream;
        private TarOutputStream _tarStream;
        private readonly IArchiveEntryTransform _entryTransform;

        public override string FileExtension => "tar.lz4";

        public Lz4ArchiveProvider(IArchiveFilePathBuilder archiveFilePathBuilder, IPasswordProvider passwordProvider)
            : base(archiveFilePathBuilder, passwordProvider)
        {
            _entryTransform = RelativePathTransform.ForTarFormat(archiveFilePathBuilder.Context.SourcePath);
        }

        public override void BeginUpdate(string sourcePath)
        {
            _fileStream = File.Create(ArchiveFilePath);

            _lz4Stream = LZ4Stream.Encode(_fileStream, level: LZ4Level.L00_FAST, leaveOpen: true);
            _tarStream = new TarOutputStream(_lz4Stream, Encoding.UTF8);
            _tarStream.IsStreamOwner = false; 
        }

        public override void Add(string path)
        {
            CompressFile(path);

            OnFileCompressed();
        }

        public override void AddDirectory(string path)
        {
            string entryName = _entryTransform.Transform(path);

            var entry = TarEntry.CreateTarEntry(entryName);
            _tarStream.PutNextEntry(entry);
            _tarStream.CloseEntry();
        }

        private void CompressFile(string filePath)
        {
            string entryName = _entryTransform.Transform(filePath);

            var entry = TarEntry.CreateTarEntry(entryName);

            using (var fs = File.OpenRead(filePath))
            {
                entry.Size = fs.Length;
                _tarStream.PutNextEntry(entry);

                fs.CopyTo(_tarStream);
                _tarStream.CloseEntry();
            }
        }

        public override void CommitUpdate()
        {
            CurrentProgressMode = ProgressMode.Indeterminate;

            try
            {
                _tarStream?.Close();
                _lz4Stream?.Dispose();
                _fileStream?.Dispose();
            }
            finally
            {
                _tarStream = null;
                _lz4Stream = null;
                _fileStream = null;
            }

            OnFinished();
        }

        public override void AbortUpdate()
        {
            try
            {
                _tarStream?.Close();
                _lz4Stream?.Dispose();
                _fileStream?.Dispose();

                if (File.Exists(ArchiveFilePath))
                {
                    File.Delete(ArchiveFilePath);
                }
            }
            catch
            { }
        }
    }
}
