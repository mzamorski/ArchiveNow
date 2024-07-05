// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace System.Extensions.IO
{

    /// <summary>
    /// Defines functionality to store and deliver search results
    /// issued during a folder analysis. This interface basically defines a
    /// list of folders and a list of files along with a couple of methods
    /// to manipulate these lists.
    /// </summary>
    public interface IDirectoryBrowsingResults
    {

        #region Properties

        /// <summary>
        /// Returns wether there is at least one folder or one file in the results.
        /// </summary>
        Boolean HasResults
        { get; }

        /// <summary>
        /// A read-only collection of <see cref="System.IO.FileInfo"/> objects
        /// created during the folder analysis.
        /// </summary>
        ReadOnlyCollection<System.IO.FileInfo> Files
        { get; }

        /// <summary>
        /// A read-only collection of <see cref="System.IO.DirectoryInfo"/> objects
        /// created during the folder analysis.
        /// </summary>
        ReadOnlyCollection<System.IO.DirectoryInfo> Directories
        { get; }

        /// <summary>
        /// The <see cref="IDirectoryBrowser"/> object that generated the
        /// results.
        /// </summary>
        IDirectoryBrowser AssociatedBrowser
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Clears the contents of the <see cref="Directories"/> and
        /// <see cref="Files"/> collections.
        /// </summary>
        void ClearResults();

        /// <summary>
        /// Appends the contents of the given <see cref="IDirectoryBrowsingResults"/>
        /// object to the internal results.
        /// </summary>
        /// <param name="results">
        /// The <see cref="IDirectoryBrowsingResults"/> to retrieve the folders and files
        /// lists.
        /// </param>
        void AddResults(IDirectoryBrowsingResults results);

        /// <summary>
        /// Appends the given folder to the results.
        /// </summary>
        /// <param name="directoryInfo">The folder to be added to the results.</param>
        void AddDirectory(System.IO.DirectoryInfo directoryInfo);

        /// <summary>
        /// Appends the given folders to the results.
        /// </summary>
        /// <param name="directoryInfos">The folders to be added to the results.</param>
        void AddDirectories(IEnumerable<System.IO.DirectoryInfo> directoryInfos);

        /// <summary>
        /// Appends the given file to the results.
        /// </summary>
        /// <param name="fileInfo">The file to be added to the results.</param>
        void AddFile(System.IO.FileInfo fileInfo);

        /// <summary>
        /// Appends the given files to the results.
        /// </summary>
        /// <param name="fileInfos">The files to be added to the results.</param>
        void AddFiles(IEnumerable<System.IO.FileInfo> fileInfos);

        #endregion

    }

}
