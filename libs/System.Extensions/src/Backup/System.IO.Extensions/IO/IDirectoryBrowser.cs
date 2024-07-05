// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

using System;
using System.Collections.ObjectModel;

namespace System.Extensions.IO
{

    /// <summary>
    /// Defines an engine that can read the contents of a disk folder and return
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
    public interface IDirectoryBrowser
    {

        #region Properties

        /// <summary>
        /// Returns wether one of the three methods
        /// <see cref="BrowseDirectory()"/>, <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior)"/>
        /// or <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior,IDirectoryBrowserSearchFilters)"/>
        /// has already been called.
        /// </summary>
        Boolean HasBrowsed
        { get; }

        /// <summary>
        /// Gets or sets the disk folder that needs to be associated with this
        /// <see cref="IDirectoryBrowser"/> instance. This is the folder that will be searched
        /// when calling one of the three methods <see cref="BrowseDirectory()"/>,
        /// <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior)"/> or
        /// <see cref="BrowseDirectory(DirectoryBrowserSearchBehavior,IDirectoryBrowserSearchFilters)"/>.
        /// </summary>
        String RootFolder
        { get; set; }

        /// <summary>
        /// Returns wether the <see cref="Errors"/> property contains at least one
        /// exception.
        /// </summary>
        Boolean HasErrors
        { get; }

        /// <summary>
        /// Returns all exceptions that may have occured during the last browsing
        /// operation. These exceptions include both exceptions that may have
        /// occured while reading the contents of the <see cref="RootFolder"/>
        /// and exceptions that may have occured in other <see cref="IDirectoryBrowser"/>
        /// child objects for child folders.
        /// </summary>
        ReadOnlyCollection<Exception> Errors
        { get;  }

        #endregion

        #region Methods

        /// <summary>
        /// Reads the contents of the <see cref="RootFolder"/> disk folder and returns
        /// the list of all its direct child folders and files. Indeed, the browser mode
        /// defaults to <see cref="DirectoryBrowserModes.ExcludeChildDirectories"/>; this
        /// behavior is inherited from the <see cref="DirectoryBrowserSearchBehavior"/>
        /// default constructor.
        /// </summary>
        /// <returns></returns>
        IDirectoryBrowsingResults BrowseDirectory();

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
        IDirectoryBrowsingResults BrowseDirectory(DirectoryBrowserSearchBehavior searchBehavior);

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
        IDirectoryBrowsingResults BrowseDirectory(DirectoryBrowserSearchBehavior searchBehavior, IDirectoryBrowserSearchFilters searchFilters);

        #endregion

    }

}
