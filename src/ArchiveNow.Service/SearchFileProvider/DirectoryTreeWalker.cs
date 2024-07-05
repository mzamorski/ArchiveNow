using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiveNow.Service.SearchFileProvider
{
    public class DirectoryTreeWalker : ISearchFileProvider
    {
        public OutputType OutputType { get; set; }

        public IEnumerable<string> GetPaths(string directoryPath)
        {
            return GetEntries(directoryPath).Select(x => x.FullName);
        }

        public IEnumerable<FileSystemInfo> GetEntries(string directoryPath)
        {
            return GetEntries(new DirectoryInfo(directoryPath));
        }

        private static IEnumerable<FileSystemInfo> GetEntries(DirectoryInfo currentDirectory)
        {
            foreach (var file in currentDirectory.GetFiles())
            {
                yield return file;
            }

            foreach (var directory in currentDirectory.GetDirectories())
            {
                yield return directory;

                foreach (var file in GetEntries(directory))
                {
                    yield return file;
                }
            }
        }

    }
}
