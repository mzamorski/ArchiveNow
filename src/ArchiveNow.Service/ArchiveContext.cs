using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

using ArchiveNow.Providers.Core;

namespace ArchiveNow.Service
{
    public class ArchiveContext
    {
        public string SourcePath { get; }
        
        public string SourceName { get; }

        public ICollection<FileSystemInfo> Entries { get; }

        public IArchiveProvider Provider { get; }

        public ArchiveContext(string sourcePathPath, ICollection<FileSystemInfo> entries, IArchiveProvider archiveProvider)
        {
            Provider = archiveProvider;
            SourcePath = sourcePathPath;
            Entries = entries ?? new Collection<FileSystemInfo>();

            SourceName = Path.GetFileNameWithoutExtension(sourcePathPath);
        }
    }
}
