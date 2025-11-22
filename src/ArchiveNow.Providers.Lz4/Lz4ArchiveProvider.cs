using System.IO;
using ArchiveNow.Providers.Core;
using ArchiveNow.Providers.Core.PasswordProviders;


namespace ArchiveNow.Providers.Lz4
{
    public class Lz4ArchiveProvider : ArchiveProviderBase
    {
        public Lz4ArchiveProvider(IArchiveFilePathBuilder archiveFilePathBuilder, IPasswordProvider passwordProvider) 
            : base(archiveFilePathBuilder, passwordProvider)
        {
            
        }

        public override string FileExtension => "lz4";

        public override void AddDirectory(string path)
        {
            foreach (var filePath in Directory.EnumerateFiles(path))
            {
                Add(filePath);
            }
        }

        public override void Add(string path)
        {

        }

        public override void BeginUpdate(string sourcePath)
        { }

        public override void CommitUpdate()
        {

        }

        public override void AbortUpdate()
        {

        }
    }
}
