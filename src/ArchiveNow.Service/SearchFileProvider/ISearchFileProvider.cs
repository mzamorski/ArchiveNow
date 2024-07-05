using System.Collections.Generic;
using System.IO;

namespace ArchiveNow.Service.SearchFileProvider
{
    public interface ISearchFileProvider
    {
        IEnumerable<string> GetPaths(string directoryPath);

        IEnumerable<FileSystemInfo> GetEntries(string directoryPath);
    }
}