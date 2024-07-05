using System.Collections.Generic;
using System.IO;

namespace ArchiveNow.Service.SearchFileProvider
{
    public class StaticSearchFileProvider : ISearchFileProvider
    {
        public IEnumerable<FileSystemInfo> GetEntries(string sourcePath)
        {
            yield return new FileInfo(sourcePath);
        }

        public IEnumerable<string> GetPaths(string sourcePath)
        {
            yield return sourcePath;
        }
    }
}
