using System;
using System.IO;
using System.Text;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;
using ArchiveNow.Utils;

using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;


namespace ArchiveNow.Service.ArchiveProviders
{
    public class SharpZipArchiveProvider : ArchiveProviderBase
    {
        private readonly IArchiveEntryTransform _entryTransform;
        private readonly ZipFile _zipFile;
        // TODO: It should be configurable.
        private readonly CompressionMethod _compressionMethod = CompressionMethod.Stored;

        public override string FileExtension => "zip";

        public SharpZipArchiveProvider(
            IArchiveFilePathBuilder archiveFilePathBuilder,
            IArchiveEntryTransform entryTransform,
            IPasswordProvider passwordProvider)
            : base(archiveFilePathBuilder, passwordProvider)
        {
            _entryTransform = entryTransform ?? NullArchiveEntryTransform.Instance;

            _zipFile = ZipFile.Create(ArchiveFilePath);
            _zipFile.NameTransform = BuildNameTransform(_entryTransform);
        }

        private static INameTransform BuildNameTransform(IArchiveEntryTransform entryTransform)
        {
            // There is no need to create native 'INameTransform' for our 'IArchiveEntryTransform' because it already exists.
            if (entryTransform is RelativePathTransform)
            {
                var directoryPath = Path.GetDirectoryName(entryTransform.RootPath);
                return new ZipNameTransform(directoryPath);
            }

            return new WindowsNameTransform();
        }

        public override void BeginUpdate()
        {
            _zipFile.BeginUpdate();

            if (Password.HasValue())
            {
                _zipFile.Password = Password;
            }
        }

        public override void CommitUpdate()
        {
            _zipFile.CommitUpdate();

            OnFinished();
        }

        public override void AbortUpdate()
        {
            _zipFile.AbortUpdate();
        }

        public override void AddDirectory(string path)
        {
            _zipFile.AddDirectory(path);
        }

        public override void Add(string path)
        {
            if (path.EndsWith("session.lock"))
            {
                _zipFile.Add(new Dupa(path), path);
                return;
            }

            _zipFile.Add(path, _compressionMethod);
        }

        public override void Dispose()
        {
            _zipFile.Close();
        }
    }

    class Dupa : IStaticDataSource
    {
        private readonly string _filePath;

        public Dupa(string filePath)
        {
            _filePath = filePath;
        }

        public Stream GetSource()
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                using (var fs = new FileStream(_filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    fs.CopyTo(ms);

                    return ms;
                }
            }
            catch (Exception ex)
            {

            }
            
            throw new InvalidOperationException();
            
        }
    }
}