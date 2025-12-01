using System.IO;
using System.IO.Compression;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.EntryTransforms;
using ArchiveNow.Providers.Core.PasswordProviders;

namespace ArchiveNow.Service.ArchiveProviders
{
    public class SystemZipArchiveProvider : ArchiveProviderBase
    {
        private readonly IArchiveEntryTransform _entryTransform;
        private readonly ZipArchive _zipArchive;

        public override string FileExtension => "zip";

        public SystemZipArchiveProvider(IArchiveFilePathBuilder archiveFilePathBuilder,
            IArchiveEntryTransform entryTransform,
            IPasswordProvider passwordProvider)
            : base(archiveFilePathBuilder, passwordProvider)
        {
            _entryTransform = entryTransform;
            _zipArchive = ZipFile.Open(ArchiveFilePath, ZipArchiveMode.Create);
        }

        public override void Dispose()
        {
            _zipArchive.Dispose();
        }

        public override void AddDirectory(string path)
        {
            var relativePath = _entryTransform.Transform(path);

            _zipArchive.CreateEntry($"{relativePath}{Path.DirectorySeparatorChar}");
        }

        /// <summary>
        /// http://stackoverflow.com/questions/15133626/creating-directories-in-a-ziparchive-c-sharp-net-4-5
        /// </summary>
        /// <param name="path"></param>
        public override void Add(string path)
        {
            var relativePath = _entryTransform.Transform(path);

            _zipArchive.CreateEntryFromFile(path, relativePath, CompressionLevel.Fastest);
        }

        public override void BeginUpdate(string sourcePath)
        { }

        public override void CommitUpdate()
        {
            _zipArchive.Dispose();
        }

        public override void AbortUpdate()
        {
            _zipArchive.Dispose();
        }
    }
}
