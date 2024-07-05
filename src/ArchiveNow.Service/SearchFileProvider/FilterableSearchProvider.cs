﻿using System.Collections.Generic;
using System.Extensions.IO;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ArchiveNow.Service.SearchFileProvider
{
    public class FilterableSearchProvider : ISearchFileProvider
    {
        private readonly DirectoryBrowserSearchBehavior _searchBehavior;
        private readonly DirectoryBrowserSearchFilters _browserFilters;

        /// <summary>
        /// Regular Expression allowed.
        /// </summary>
        /// <param name="ignoredDirectories"></param>
        /// <param name="ignoredFiles"></param>
        public FilterableSearchProvider(ICollection<string> ignoredDirectories, ICollection<string> ignoredFiles)
        {
            _searchBehavior = new DirectoryBrowserSearchBehavior
            {
                BrowserMode = DirectoryBrowserModes.SearchOnlyMatchingDirectories,
                DirectoryMatchRules = DirectoryMatchRules.IncludeMatchingDirectoryContents
            };

            var directoryFilter = DirectoryBrowserSearchFilters.CreateDirectoryInfoNameSearchFilter(ignoredDirectories,
                SearchOptions.ExcludeMatches);
            directoryFilter.MatchMode = FileSystemInfoNameMatchMode.UseRegEx;
            
            var fileFilter = DirectoryBrowserSearchFilters.CreateFileInfoNameSearchFilter(ignoredFiles,
                SearchOptions.ExcludeMatches);
            fileFilter.MatchMode = FileSystemInfoNameMatchMode.UseRegEx;

            _browserFilters = new DirectoryBrowserSearchFilters(directoryFilter, fileFilter);
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>List of filtered full paths (read only)</returns>
        public IEnumerable<string> GetPaths(string directoryPath)
        {
            return GetEntries(directoryPath).Select(x => x.FullName);
        }

        /// <summary>
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns>List of filtered files (read only)</returns>
        public IEnumerable<FileSystemInfo> GetEntries(string directoryPath)
        {
            var result = DirectoryBrowser.BrowseDirectory(directoryPath, _searchBehavior, _browserFilters);

            return (
                new ReadOnlyCollectionBuilder<FileSystemInfo>(
                    result.Directories
                    .Cast<FileSystemInfo>()
                    .Concat(result.Files)
                )
            ).ToReadOnlyCollection();
        }
    }
}