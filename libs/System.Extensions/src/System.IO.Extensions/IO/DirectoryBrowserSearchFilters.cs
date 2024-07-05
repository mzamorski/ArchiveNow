// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

using System;
using System.Collections.Generic;
using System.IO;

namespace System.Extensions.IO
{

    /// <summary>
    /// Encapsulates one <see cref="SearchFilter{DirectoryInfo}"/> object and one <see cref="SearchFilter{FileInfo}"/>
    /// object for a <see cref="IDirectoryBrowser"/> object that needs to be able to process folders and files independently.
    /// You are not restricted to the use of one  single search filter for all folders and  another single search filter for all files,
    /// stacking multiple <see cref="ISearchFilter{T}"/> filter objects  for folders and/or files is possible when using a type
    /// deriving from <see cref="SearchFilter{T}"/> because it implements a Decorator pattern.
    /// </summary>
    public class DirectoryBrowserSearchFilters
        : IDirectoryBrowserSearchFilters
    {

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes the new <see cref="DirectoryBrowserSearchFilters"/> object using
        /// empty instances of <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/>
        /// and <see cref="FileSystemInfoNameSearchFilter{FileInfo}"/>. The <see cref="DirectoryFilter"/>
        /// and <see cref="FileFilter"/> properties can be used to access them and set their patterns
        /// respectively.
        /// </summary>
        public DirectoryBrowserSearchFilters()
            : this(new FileSystemInfoNameSearchFilter<DirectoryInfo>(),
            new FileSystemInfoNameSearchFilter<FileInfo>())
        {
        }

        /// <summary>
        /// Initializes the new <see cref="DirectoryBrowserSearchFilters"/> object using
        /// the provided <see cref="ISearchFilter{DirectoryInfo}"/> and
        /// <see cref="ISearchFilter{FileInfo}"/> filters.
        /// </summary>
        /// <param name="directoryFilter">
        /// A <see cref="ISearchFilter{DirectoryInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a folder should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{DirectoryInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </param>
        /// <param name="fileFilter">
        /// A <see cref="ISearchFilter{FileInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a file should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{FileInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </param>
        public DirectoryBrowserSearchFilters(ISearchFilter<DirectoryInfo> directoryFilter, ISearchFilter<FileInfo> fileFilter)
        {
            this.DirectoryFilter = directoryFilter;
            this.FileFilter = fileFilter;
        }

        #endregion

        #region IDirectoryBrowserPatterns

        /// <summary>
        /// A <see cref="ISearchFilter{DirectoryInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a folder should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{DirectoryInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </summary>
        public ISearchFilter<DirectoryInfo> DirectoryFilter
        { get; set; }

        /// <summary>
        /// A <see cref="ISearchFilter{FileInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a file should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{FileInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </summary>
        public ISearchFilter<FileInfo> FileFilter
        { get; set; }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Returns an instance of <see cref="DirectoryBrowserSearchFilters"/> using the provided
        /// <see cref="ISearchFilter{DirectoryInfo}"/> and <see cref="ISearchFilter{FileInfo}"/> filters.
        /// </summary>
        /// <param name="directorySearchFilter">
        /// A <see cref="ISearchFilter{DirectoryInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a folder should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{DirectoryInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </param>
        /// <param name="fileSearchFilter">
        /// A <see cref="ISearchFilter{FileInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a file should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{FileInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowserSearchFilters CreateNew(ISearchFilter<DirectoryInfo> directorySearchFilter, ISearchFilter<FileInfo> fileSearchFilter)
        {
            return new DirectoryBrowserSearchFilters(directorySearchFilter, fileSearchFilter);
        }

        /// <summary>
        /// Splits a string containing substrings separated by a comma and returns the substrings
        /// as a list.
        /// </summary>
        /// <param name="commaSeparatedPatterns">A string containing substrings separated by a comma.</param>
        /// <returns></returns>
        public static ICollection<String> CreatePatternsList(String commaSeparatedPatterns)
        {
            List<String> patterns;

            if (!String.IsNullOrEmpty(commaSeparatedPatterns))
            {
                patterns = new List<String>(commaSeparatedPatterns.Replace(" ",String.Empty).Split(','));
            }
            else
            {
                patterns = new List<String>();
            }

            return patterns;
        }

        /// <summary>
        /// Returns an instance of <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/>
        /// using the given comma-separated string wildcard patterns or Regular Expressions.
        /// </summary>
        /// <param name="commaSeparatedPatterns">
        /// Comma-separated string wildcard patterns  or Regular Expressions that will be split
        /// for creating the patterns list.
        /// </param>
        /// <param name="searchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static FileSystemInfoNameSearchFilter<DirectoryInfo> CreateDirectoryInfoNameSearchFilter(String commaSeparatedPatterns, SearchOptions searchMode)
        {
            return new FileSystemInfoNameSearchFilter<DirectoryInfo>(searchMode, CreatePatternsList(commaSeparatedPatterns));
        }

        /// <summary>
        /// Returns an instance of <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/>
        /// using the given list of string wildcard patterns or Regular Expressions.
        /// </summary>
        /// <param name="patterns">
        /// The patterns list.
        /// </param>
        /// <param name="searchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static FileSystemInfoNameSearchFilter<DirectoryInfo> CreateDirectoryInfoNameSearchFilter(ICollection<String> patterns, SearchOptions searchMode)
        {
            return new FileSystemInfoNameSearchFilter<DirectoryInfo>(searchMode, patterns);
        }

        /// <summary>
        /// Returns an instance of <see cref="FileSystemInfoNameSearchFilter{FileInfo}"/>
        /// using the given comma-separated string wildcard patterns or Regular Expressions.
        /// </summary>
        /// <param name="commaSeparatedPatterns">
        /// Comma-separated string wildcard patterns  or Regular Expressions that will be
        /// split for creating the patterns list.
        /// </param>
        /// <param name="searchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static FileSystemInfoNameSearchFilter<FileInfo> CreateFileInfoNameSearchFilter(String commaSeparatedPatterns, SearchOptions searchMode)
        {
            return new FileSystemInfoNameSearchFilter<FileInfo>(searchMode, CreatePatternsList(commaSeparatedPatterns));
        }

        /// <summary>
        /// Returns an instance of <see cref="FileSystemInfoNameSearchFilter{FileInfo}"/>
        /// using the given list of string wildcard patterns or Regular Expressions.
        /// </summary>
        /// <param name="patterns">
        /// The patterns list.
        /// </param>
        /// <param name="searchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static FileSystemInfoNameSearchFilter<FileInfo> CreateFileInfoNameSearchFilter(ICollection<String> patterns, SearchOptions searchMode)
        {
            return new FileSystemInfoNameSearchFilter<FileInfo>(searchMode, patterns);
        }

        #endregion

    }

}
