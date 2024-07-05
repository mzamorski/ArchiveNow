// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace System.Extensions.IO
{

    /// <summary>
    /// An engine that can read the contents of a disk folder and return
    /// a list of folders and files having positive matches for one or multiple
    /// <see cref="ISearchFilter{T}"/> objects using one of the various introspection
    /// techniques defined by a <see cref="DirectoryBrowserSearchBehavior"/> object.
    /// </summary>
    /// <remarks>
    /// The <see cref="ISearchFilter{T}"/> interface has been left generic enough to
    /// permit any kind of search/comparison. Default concrete implementations like
    /// <see cref="FileSystemInfoNameSearchFilter{T}"/> and
    /// <see cref="FileSystemInfoAttributeSearchFilter{T}"/> are provided to validate
    /// folders and files against pre-determined patterns and attributes but you could
    /// perfectly make a new class also inheriting from the <see cref="SearchFilter{T}"/>
    /// base class (to inherit  a Decorator behavior and allow stacking of multiple filters)
    /// that would for example  be able to look inside files for a given string. Such a
    /// sample implementation actually exists, the <see cref="FileInfoContentsSearchFilter"/>
    /// shows a very basic implementation that can search for a particular string in files.
    /// </remarks>
    public class DirectoryBrowser
        : IDirectoryBrowser
    {

        #region Private Fieds

        private String _rootFolder;
        private DirectoryInfo _rootFolderInfo;
        private List<Exception> _exceptionsList;
        private ReadOnlyCollection<Exception> _exceptionsCollection;

        #endregion

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes the browser.
        /// </summary>
        /// <param name="rootFolder"></param>
        public DirectoryBrowser(String rootFolder)
        {
            this.RootFolder = rootFolder;
            this.Results = new DirectoryBrowsingResults(this, this.RootFolder);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns wether the <see cref="RootFolder"/> property
        /// contents refers to a valid disk folder.
        /// </summary>
        public Boolean RootFolderExists
        {
            get { return Directory.Exists(this.RootFolder); }
        }

        /// <summary>
        /// Returns the results associated with the last call to
        /// <see cref="BrowseDirectory()"/>, <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior)"/>
        /// or <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior,IDirectoryBrowserSearchFilters)"/>.
        /// The static methods are implictely included because they make use of one of these instance methods.
        /// </summary>
        public IDirectoryBrowsingResults Results
        { get; private set; }

        /// <summary>
        /// Returns wether the <see cref="Results"/> object contains
        /// results, that's to say at least one folder or one file. Results can
        /// be empty when the last search did not return elements matching the
        /// <see cref="IDirectoryBrowserSearchFilters"/> criterias, or when no
        /// search has been done yet. It is possible to distinguish both cases
        /// checking the <see cref="HasBrowsed"/> property.
        /// </summary>
        public Boolean HasResults
        {
            get
            {
                if (this.Results != null)
                {
                    return this.Results.HasResults;
                }
                return false;
            }
        }

        #endregion

        #region Private Properties

        /// <summary>
        /// Returns a <see cref="DirectoryInfo"/> object for the folder
        /// defined by the <see cref="RootFolder"/> property contents.
        /// </summary>
        private DirectoryInfo RootFolderInfo
        {
            get
            {
                if (this._rootFolderInfo == null)
                    this._rootFolderInfo = new DirectoryInfo(this.RootFolder);

                return this._rootFolderInfo;
            }
        }

        /// <summary>
        /// Returns all exceptions that may have occured during the last browsing
        /// operation as a List internally for feeding the public
        /// read-only <see cref="Errors"/> property.
        /// </summary>
        private List<Exception> ExceptionsList
        {
            get
            {
                if (this._exceptionsList == null)
                    this._exceptionsList = new List<Exception>();

                return this._exceptionsList;
            }
        }

        #endregion

        #region IDirectoryBrowser

        /// <summary>
        /// Returns wether one of the three methods
        /// <see cref="BrowseDirectory()"/>, <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior)"/>
        /// or <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior,IDirectoryBrowserSearchFilters)"/>
        /// has already been called. The static methods are implictely included because they make use
        /// of one of these instance methods.
        /// </summary>
        public Boolean HasBrowsed
        { get; private set; }

        /// <summary>
        /// Gets or sets the disk folder that needs to be associated with this
        /// <see cref="IDirectoryBrowser"/> instance. This is the folder that will be searched
        /// when calling one of the three methods <see cref="BrowseDirectory()"/>,
        /// <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior)"/> or
        /// <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior,IDirectoryBrowserSearchFilters)"/>.
        /// The static methods are implictely included because they make use of one of these instance methods.
        /// </summary>
        public String RootFolder
        {
            get { return this._rootFolder; }
            set { this._rootFolder = value; }
        }

        /// <summary>
        /// Returns all exceptions that may have occured during the last browsing
        /// operation. These exceptions include both exceptions that may have
        /// occured while reading the contents of the <see cref="RootFolder"/>
        /// and exceptions that may have occured in other <see cref="IDirectoryBrowser"/>
        /// child objects for child folders.
        /// </summary>
        public ReadOnlyCollection<Exception> Errors
        {
            get
            {
                if (this._exceptionsCollection == null)
                    this._exceptionsCollection = new ReadOnlyCollection<Exception>(this.ExceptionsList);

                return this._exceptionsCollection;
            }
        }

        /// <summary>
        /// Returns wether the <see cref="Errors"/> property contains at least one
        /// exception.
        /// </summary>
        public Boolean HasErrors
        {
            get { return this.Errors.Count != 0; }
        }

        /// <summary>
        /// Reads the contents of the <see cref="RootFolder"/> disk folder and returns
        /// the list of all its direct child folders and files. Indeed, the browser mode
        /// defaults to <see cref="DirectoryBrowserModes.ExcludeChildDirectories"/>; this
        /// behavior is inherited from the <see cref="DirectoryBrowserSearchBehavior"/>
        /// default constructor.
        /// </summary>
        /// <returns></returns>
        public IDirectoryBrowsingResults BrowseDirectory()
        {
            return this.BrowseDirectory(null);
        }

        /// <summary>
        /// Reads the contents of the <see cref="RootFolder"/> disk folder and returns
        /// a list of folders and files using one of the various introspection
        /// techniques defined by a <see cref="DirectoryBrowserSearchBehavior"/> object.
        /// When no <see cref="ISearchFilter{T}"/> objects are provided via a
        /// <see cref="IDirectoryBrowserSearchFilters"/> object, all files and folders are considered
        /// as matching. Therefore, the <see cref="DirectoryBrowserSearchBehavior"/> can just be
        /// used to include or exclude the contents of child folders.
        /// </summary>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not.
        /// </param>
        /// <returns></returns>
        public IDirectoryBrowsingResults BrowseDirectory(DirectoryBrowserSearchBehavior searchBehavior)
        {
            return this.BrowseDirectory(searchBehavior, null);
        }

        /// <summary>
        /// Reads the contents of the <see cref="RootFolder"/> disk folder and returns
        /// a list of folders and files having positive matches for one or multiple
        /// <see cref="ISearchFilter{T}"/> objects using one of the various introspection
        /// techniques defined by a <see cref="DirectoryBrowserSearchBehavior"/> object.
        /// </summary>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="searchFilters">
        /// A <see cref="IDirectoryBrowser"/> object encapsulating one <see cref="SearchFilter{DirectoryInfo}"/>
        /// and one <see cref="SearchFilter{FileInfo}"/> for being able to process folders and files
        /// independently. You are not restricted to the use of one single search filter for all folders and
        /// another single search filter for all files, stacking multiple <see cref="ISearchFilter{T}"/> filter objects
        /// for folders and/or files is possible when using a type deriving from <see cref="SearchFilter{T}"/>
        /// because it implements a Decorator pattern. When a filter is null or empty, all folders/files are
        /// considered as matching.
        /// </param>
        /// <returns></returns>
        public IDirectoryBrowsingResults BrowseDirectory(DirectoryBrowserSearchBehavior searchBehavior, IDirectoryBrowserSearchFilters searchFilters)
        {
            DirectoryBrowserSearchBehavior behavior = searchBehavior ?? new DirectoryBrowserSearchBehavior();

            this.Results.ClearResults();
            this.ExceptionsList.Clear();
            this.HasBrowsed = true;

            if (this.RootFolderExists)
            {
                IEnumerable<DirectoryInfo> allDirectories;
                IEnumerable<DirectoryInfo> matchingDirectories = this.GetDirectories(searchFilters);
                IEnumerable<FileInfo> files = this.GetFiles(searchFilters);

                if (behavior.BrowserMode == DirectoryBrowserModes.SearchOnlyMatchingDirectories)
                {
                    if (behavior.DirectoryMatchRules == DirectoryMatchRules.IncludeCompleteDirectoryContents)
                    {
                        this.Results.AddResults(this.BrowseChildDirectories(matchingDirectories, searchBehavior, null));
                    }
                    else
                    {
                        this.Results.AddResults(this.BrowseChildDirectories(matchingDirectories, searchBehavior, searchFilters));
                    }
                }
                else if (behavior.BrowserMode == DirectoryBrowserModes.SearchAllChildDirectories)
                {
                    allDirectories = this.GetDirectories(null);
                    if (behavior.DirectoryMatchRules == DirectoryMatchRules.IncludeCompleteDirectoryContents)
                    {
                        // Search all directories except the matching ones using the filters
                        this.Results.AddResults(this.BrowseChildDirectories(
                            allDirectories.Except(matchingDirectories, new DirectoryInfoEqualityComparerByFullName()),
                            searchBehavior, searchFilters));
                        // Then search the matching directories without filter to include their complete contents
                        this.Results.AddResults(this.BrowseChildDirectories(matchingDirectories, searchBehavior, null));
                    }
                    else
                    {
                        // Search all directories using the filters
                        this.Results.AddResults(this.BrowseChildDirectories(allDirectories, searchBehavior, searchFilters));
                    }
                }

                this.Results.AddDirectories(matchingDirectories);
                this.Results.AddFiles(files);
            }

            return this.Results;
        }

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Instantiates a <see cref="DirectoryBrowser"/> object and calls one of its
        /// <see cref="IDirectoryBrowser"/> methods to search the contents of the <see cref="RootFolder"/>
        /// using the arguments provided.
        /// The <see cref="IDirectoryBrowser"/> instance is referenced in the
        /// <see cref="IDirectoryBrowsingResults.AssociatedBrowser"/> property and can be used to
        /// access eventual exceptions that may have occured during processing.
        /// </summary>
        /// <param name="folderPath">The root folder path, on which searches will be applied.</param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowsingResults BrowseDirectory(String folderPath, DirectoryBrowserSearchBehavior searchBehavior)
        {
            return CreateBrowser(folderPath).BrowseDirectory(searchBehavior);
        }

        /// <summary>
        /// Instantiates a <see cref="DirectoryBrowser"/> object and calls one of its
        /// <see cref="IDirectoryBrowser"/> methods to search the contents of the <see cref="RootFolder"/>
        /// using the arguments provided.
        /// The <see cref="IDirectoryBrowser"/> instance is referenced in the
        /// <see cref="IDirectoryBrowsingResults.AssociatedBrowser"/> property and can be used to
        /// access eventual exceptions that may have occured during processing.
        /// </summary>
        /// <param name="folderPath">The root folder path, on which searches will be applied.</param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="searchFilters">
        /// A <see cref="IDirectoryBrowser"/> object encapsulating one <see cref="SearchFilter{DirectoryInfo}"/>
        /// and one <see cref="SearchFilter{FileInfo}"/> for being able to process folders and files
        /// independently. You are not restricted to the use of one single search filter for all folders and
        /// another single search filter for all files, stacking multiple <see cref="ISearchFilter{T}"/> filter objects
        /// for folders and/or files is possible when using a type deriving from <see cref="SearchFilter{T}"/>
        /// because it implements a Decorator pattern. When a filter is null or empty, all folders/files are
        /// considered as matching.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowsingResults BrowseDirectory(String folderPath, DirectoryBrowserSearchBehavior searchBehavior, IDirectoryBrowserSearchFilters searchFilters)
        {
            return CreateBrowser(folderPath).BrowseDirectory(searchBehavior, searchFilters);
        }

        /// <summary>
        /// Instantiates a <see cref="DirectoryBrowser"/> object and calls one of its
        /// <see cref="IDirectoryBrowser"/> methods to search the contents of the <see cref="RootFolder"/>
        /// using the arguments provided.
        /// The <see cref="IDirectoryBrowser"/> instance is referenced in the
        /// <see cref="IDirectoryBrowsingResults.AssociatedBrowser"/> property and can be used to
        /// access eventual exceptions that may have occured during processing.
        /// </summary>
        /// <param name="folderPath">The root folder path, on which searches will be applied.</param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="directorySearchFilter">
        /// A <see cref="ISearchFilter{DirectoryInfo}"/> object for creating a
        /// <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <param name="fileSearchFilter">
        /// A <see cref="ISearchFilter{FileInfo}"/> object for creating a
        /// <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowsingResults BrowseDirectory(String folderPath, DirectoryBrowserSearchBehavior searchBehavior,
            ISearchFilter<DirectoryInfo> directorySearchFilter, ISearchFilter<FileInfo> fileSearchFilter)
        {
            return BrowseDirectory(folderPath, searchBehavior,
                                   DirectoryBrowserSearchFilters.CreateNew(directorySearchFilter, fileSearchFilter));
        }

        /// <summary>
        /// Instantiates a <see cref="DirectoryBrowser"/> object and calls one of its
        /// <see cref="IDirectoryBrowser"/> methods to search the contents of the <see cref="RootFolder"/>
        /// using the arguments provided.
        /// The <see cref="IDirectoryBrowser"/> instance is referenced in the
        /// <see cref="IDirectoryBrowsingResults.AssociatedBrowser"/> property and can be used to
        /// access eventual exceptions that may have occured during processing.
        /// </summary>
        /// <param name="folderPath">The root folder path, on which searches will be applied.</param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="directoryPatterns">
        /// A list of String patterns using wildcards or Regular Expressions for creating a
        /// <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/> filter object, to be used
        /// as default <see cref="ISearchFilter{DirectoryInfo}"/> while constructing the
        /// <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <param name="directorySearchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="filePatterns">
        /// A list String patterns using wildcards or Regular Expressions for creating a
        /// <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/> filter object, to be used
        /// as default <see cref="ISearchFilter{FileInfo}"/> while constructing the
        /// <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <param name="fileSearchMode">
        /// Specifies wether file matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowsingResults BrowseDirectory(String folderPath, DirectoryBrowserSearchBehavior searchBehavior,
            ICollection<String> directoryPatterns, SearchOptions directorySearchMode,
            ICollection<String> filePatterns, SearchOptions fileSearchMode)
        {
            return BrowseDirectory(folderPath, searchBehavior,
                                            DirectoryBrowserSearchFilters.CreateDirectoryInfoNameSearchFilter(directoryPatterns, directorySearchMode),
                                            DirectoryBrowserSearchFilters.CreateFileInfoNameSearchFilter(filePatterns, fileSearchMode));
        }

        /// <summary>
        /// Instantiates a <see cref="DirectoryBrowser"/> object and calls one of its
        /// <see cref="IDirectoryBrowser"/> methods to search the contents of the <see cref="RootFolder"/>
        /// using the arguments provided.
        /// The <see cref="IDirectoryBrowser"/> instance is referenced in the
        /// <see cref="IDirectoryBrowsingResults.AssociatedBrowser"/> property and can be used to
        /// access eventual exceptions that may have occured during processing.
        /// </summary>
        /// <param name="folderPath">The root folder path, on which searches will be applied.</param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="commaSeparatedDirectoryPatterns">
        /// Comma-separated String patterns using wildcards or Regular Expressions that will be split
        /// for creating a <see cref="FileSystemInfoNameSearchFilter{DirectoryInfo}"/>
        /// filter object to be used as default <see cref="ISearchFilter{DirectoryInfo}"/> while constructing
        /// the <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <param name="directorySearchMode">
        /// Specifies wether folder matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="commaSeparatedFilePatterns">
        /// Comma-separated String patterns using wildcards or Regular Expressions that will be split
        /// for creating a <see cref="FileSystemInfoNameSearchFilter{FileInfo}"/>
        /// filter object to be used as default <see cref="ISearchFilter{FileInfo}"/> while constructing
        /// the <see cref="IDirectoryBrowserSearchFilters"/> object.
        /// </param>
        /// <param name="fileSearchMode">
        /// Specifies wether file matches should be held as parts of the results, or rejected.
        /// </param>
        /// <returns></returns>
        public static IDirectoryBrowsingResults BrowseDirectory(String folderPath, DirectoryBrowserSearchBehavior searchBehavior,
            String commaSeparatedDirectoryPatterns, SearchOptions directorySearchMode,
            String commaSeparatedFilePatterns, SearchOptions fileSearchMode)
        {
            return BrowseDirectory(folderPath, searchBehavior,
                                            DirectoryBrowserSearchFilters.CreateDirectoryInfoNameSearchFilter(commaSeparatedDirectoryPatterns, directorySearchMode),
                                            DirectoryBrowserSearchFilters.CreateFileInfoNameSearchFilter(commaSeparatedFilePatterns, fileSearchMode));
        }

        /// <summary>
        /// Returns a new DirectoryBrowser instance.
        /// </summary>
        /// <param name="rootFolder">The root folder to search.</param>
        /// <returns></returns>
        public static DirectoryBrowser CreateBrowser(String rootFolder)
        {
            return new DirectoryBrowser(rootFolder);
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Searches the contents of each directory according to the specified
        /// search behavior and filters.
        /// </summary>
        /// <param name="directories">
        /// The list of directories to search. A new <see cref="DirectoryBrowser"/> instance
        /// will be created for each <see cref="DirectoryInfo"/> object in the list. Their
        /// <see cref="IDirectoryBrowsingResults"/> and eventual <see cref="Errors"/> will
        /// be added to their counterparts in the current instance.
        /// </param>
        /// <param name="searchBehavior">
        /// A <see cref="DirectoryBrowserSearchBehavior"/> object to specify wether child folders
        /// should also be searched or not. Child folders can be excluded or included idependently
        /// using one or more <see cref="ISearchFilter{T}"/> objects, and they can be searched
        /// to return all of their contents or just filtered contents when they match themselves
        /// directory search criterias.
        /// </param>
        /// <param name="searchFilters">
        /// Contains the <see cref="ISearchFilter{DirectoryInfo}"/> and <see cref="ISearchFilter{FileInfo}"/> filters
        /// to apply separately on folders and files during the search. Multiple search filters can be combined by using types
        /// deriving from <see cref="SearchFilter{T}"/> because this base class implements a Decorator pattern.
        /// </param>
        /// <returns></returns>
        private IDirectoryBrowsingResults BrowseChildDirectories(IEnumerable<DirectoryInfo> directories, DirectoryBrowserSearchBehavior searchBehavior,
            IDirectoryBrowserSearchFilters searchFilters)
        {
            IDirectoryBrowsingResults results = new DirectoryBrowsingResults(this, this.RootFolder);

            if (directories != null)
            {
                foreach (DirectoryInfo dir in directories)
                {
                    IDirectoryBrowser browser = CreateBrowser(dir.FullName);
                    results.AddResults(browser.BrowseDirectory(searchBehavior, searchFilters));
                    if (browser.HasErrors)
                    {
                        this.HandleExceptions(browser.Errors);
                    }
                }
            }

            return results;
        }

        /// <summary>
        /// Gets all direct child folders under the <see cref="RootFolder"/> having successfully
        /// passed the search filters. When the <see cref="IDirectoryBrowserSearchFilters"/> object
        /// is null, or when its <see cref="IDirectoryBrowserSearchFilters.DirectoryFilter"/> is null
        /// or empty, all directories are returned.
        /// </summary>
        /// <param name="searchFilters">
        /// Contains the <see cref="ISearchFilter{DirectoryInfo}"/> to apply separately on folders during the search.
        /// Multiple search filters can be combined by using types deriving from <see cref="SearchFilter{T}"/>
        /// because this base class implements a Decorator pattern.
        /// </param>
        /// <returns></returns>
        protected IEnumerable<DirectoryInfo> GetDirectories(IDirectoryBrowserSearchFilters searchFilters)
        {
            try
            {
                if (searchFilters == null || searchFilters.DirectoryFilter == null || !searchFilters.DirectoryFilter.ContainsFilterCriterias)
                    return this.RootFolderInfo.GetDirectories();

                // We provide an error handler to the search filter Matches() method
                // to prevent a single Matches() instruction to crash the complete predicate.
                // We would otherwise loose the complete set of results for the folder.
                // The search filter is expected to swallow exceptions, but also to make use of the
                // delegate to register errors.
                return this.RootFolderInfo.GetDirectories().Where(dirInfo => searchFilters.DirectoryFilter.Matches(dirInfo, this.HandleException));
            }
            catch (Exception ex)
            {
                // Probably security exception due to an unauthorized access
                // to the folder.
                this.HandleException(ex);
                return new List<DirectoryInfo>();
            }
        }

        /// <summary>
        /// Gets all direct child files under the <see cref="RootFolder"/> having successfully
        /// passed the search filters. When the <see cref="IDirectoryBrowserSearchFilters"/> object
        /// is null, or when its <see cref="IDirectoryBrowserSearchFilters.FileFilter"/> is null
        /// or empty, all directories are returned.
        /// </summary>
        /// <param name="searchFilters">
        /// Contains the <see cref="ISearchFilter{FileInfo}"/> to apply separately on files during the search.
        /// Multiple search filters can be combined by using types deriving from <see cref="SearchFilter{T}"/>
        /// because this base class implements a Decorator pattern.
        /// </param>
        /// <returns></returns>
        protected IEnumerable<FileInfo> GetFiles(IDirectoryBrowserSearchFilters searchFilters)
        {
            try
            {
                if (searchFilters == null || searchFilters.FileFilter == null || !searchFilters.FileFilter.ContainsFilterCriterias)
                    return this.RootFolderInfo.GetFiles();

                // We provide an error handler to the search filter Matches() method
                // to prevent a single Matches() instruction to crash the complete predicate.
                // We would otherwise loose the complete set of results for the folder.
                // The search filter is expected to swallow exceptions, but also to make use of the
                // delegate to register errors.
                return this.RootFolderInfo.GetFiles().Where(fileInfo => searchFilters.FileFilter.Matches(fileInfo, this.HandleException));
            }
            catch (Exception ex)
            {
                // Probably security exception due to an unauthorized access
                // to the folder.
                this.HandleException(ex);
                return new List<FileInfo>();
            }
        }

        /// <summary>
        /// Adds the exceptions to the internal exceptions list.
        /// </summary>
        /// <param name="exceptions"></param>
        protected virtual void HandleExceptions(IEnumerable<Exception> exceptions)
        {
            if (exceptions == null)
                return;

            this.ExceptionsList.AddRange(exceptions);
        }

        /// <summary>
        /// Adds the exception to the internal exceptions list.
        /// </summary>
        /// <param name="exception"></param>
        protected virtual void HandleException(Exception exception)
        {
            if (exception == null)
                return;
            
            this.ExceptionsList.Add(exception);
        }

        #endregion

    }

}
