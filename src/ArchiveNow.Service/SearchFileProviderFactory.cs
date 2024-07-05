using System.Collections.Generic;
using System.Linq;
using ArchiveNow.Service.SearchFileProvider;
using ArchiveNow.Utils;

namespace ArchiveNow.Service
{
    public class SearchFileProviderFactory
    {
        public static ISearchFileProvider Build()
        {
            return new DirectoryTreeWalker();
        }

        public static ISearchFileProvider Build(ICollection<string> ignoredDirectories, ICollection<string> ignoredFiles)
        {
            if (!HasAnyExclusions(ignoredDirectories, ignoredFiles))
            {
                return Build();
            }

            return new FilterableSearchProvider(ignoredDirectories, ignoredFiles);
        }

        private static bool HasAnyExclusions(IEnumerable<string> ignoredDirectories, IEnumerable<string> ignoredFiles)
        {
            return (ignoredDirectories.Any()) || (ignoredFiles.Any());
        }
    }
}