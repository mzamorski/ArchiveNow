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
    /// Encapsulates functionality to store and deliver search results
    /// issued during a folder analysis. This class basically stores a
    /// list of folders and a list of files, and offers a couple of methods
    /// to manipulate these lists.
    /// </summary>
    public class DirectoryBrowsingResults
        : IDirectoryBrowsingResults
    {

        #region Private Fields

        private ReadOnlyCollection<System.IO.FileInfo> _filesCollection;
        private ReadOnlyCollection<System.IO.DirectoryInfo> _directoriesCollection;

        #endregion

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes the instance with references to the given <see cref="IDirectoryBrowser"/>
        /// and root folder path.
        /// </summary>
        /// <param name="associatedBrowser">
        /// The <see cref="IDirectoryBrowser"/> object that requested an instantiation of this
        /// results object.
        /// </param>
        /// <param name="rootFolder">
        /// The path of the folder that was used as root folder for the analysis.
        /// </param>
        public DirectoryBrowsingResults(IDirectoryBrowser associatedBrowser, String rootFolder)
        {
            this.FilesList = new List<System.IO.FileInfo>();
            this.DirectoriesList = new List<System.IO.DirectoryInfo>();
            this.RootFolder = rootFolder;
            this.AssociatedBrowser = associatedBrowser;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the path of the folder that was passed to the
        /// <see cref="AssociatedBrowser"/> object that generated the results.
        /// </summary>
        public String RootFolder
        { get; private set; }

        #endregion

        #region Private Properties

        /// <summary>
        /// The internal list of DirectoryInfo objects created during the folder
        /// analysis. This list is encapsulated in the <see cref="Directories"/>
        /// property (of type <see cref="ReadOnlyCollection{DirectoryInfo}"/>).
        /// </summary>
        private List<System.IO.DirectoryInfo> DirectoriesList
        { get; set; }

        /// <summary>
        /// The internal list of FileInfo objects created during the folder
        /// analysis. This list is encapsulated in the <see cref="Files"/>
        /// property (of type <see cref="ReadOnlyCollection{FileInfo}"/>).
        /// </summary>
        private List<System.IO.FileInfo> FilesList
        { get; set; }

        #endregion

        #region IDirectoryBrowsingResults

        /// <summary>
        /// A read-only collection of <see cref="System.IO.FileInfo"/> objects
        /// created during the folder analysis.
        /// </summary>
        public ReadOnlyCollection<System.IO.FileInfo> Files
        {
            get
            {
                if (this._filesCollection == null)
                    this._filesCollection = new ReadOnlyCollection<System.IO.FileInfo>(this.FilesList);

                return this._filesCollection;
            }
        }

        /// <summary>
        /// A read-only collection of <see cref="System.IO.DirectoryInfo"/> objects
        /// created during the folder analysis.
        /// </summary>
        public ReadOnlyCollection<System.IO.DirectoryInfo> Directories
        {
            get
            {
                if (this._directoriesCollection == null)
                    this._directoriesCollection = new ReadOnlyCollection<System.IO.DirectoryInfo>(this.DirectoriesList);

                return this._directoriesCollection;
            }
        }

        /// <summary>
        /// Returns wether there is at least one folder or one file in the results.
        /// </summary>
        public Boolean HasResults
        {
            get 
            {
                return (this.Directories.Count != 0 || this.Files.Count != 0);
            }
        }

        /// <summary>
        /// The <see cref="IDirectoryBrowser"/> object that generated the
        /// results.
        /// </summary>
        public IDirectoryBrowser AssociatedBrowser
        { get; private set; }

        /// <summary>
        /// Clears the contents of the <see cref="Directories"/> and
        /// <see cref="Files"/> collections.
        /// </summary>
        public void ClearResults()
        {
            this.DirectoriesList.Clear();
            this.FilesList.Clear();
        }

        /// <summary>
        /// Appends the contents of the given <see cref="IDirectoryBrowsingResults"/>
        /// object to the internal results.
        /// </summary>
        /// <param name="results">
        /// The <see cref="IDirectoryBrowsingResults"/> to retrieve the folders and files
        /// lists.
        /// </param>
        public void AddResults(IDirectoryBrowsingResults results)
        {
            if (results == null || !results.HasResults)
                return;

            this.DirectoriesList.AddRange(results.Directories);
            this.FilesList.AddRange(results.Files);
        }

        /// <summary>
        /// Appends the given folders to the results.
        /// </summary>
        /// <param name="directoryInfos">The folders to be added to the results.</param>
        public void AddDirectories(IEnumerable<System.IO.DirectoryInfo> directoryInfos)
        {
            if (directoryInfos == null)
                return;

            this.DirectoriesList.AddRange(directoryInfos);
        }

        /// <summary>
        /// Appends the given folder to the results.
        /// </summary>
        /// <param name="directoryInfo">The folder to be added to the results.</param>
        public void AddDirectory(System.IO.DirectoryInfo directoryInfo)
        {
            if (directoryInfo == null)
                return;

            this.DirectoriesList.Add(directoryInfo);
        }

        /// <summary>
        /// Appends the given files to the results.
        /// </summary>
        /// <param name="fileInfos">The files to be added to the results.</param>
        public void AddFiles(IEnumerable<System.IO.FileInfo> fileInfos)
        {
            if (fileInfos == null)
                return;

            this.FilesList.AddRange(fileInfos);
        }

        /// <summary>
        /// Appends the given file to the results.
        /// </summary>
        /// <param name="fileInfo">The file to be added to the results.</param>
        public void AddFile(System.IO.FileInfo fileInfo)
        {
            if (fileInfo == null)
                return;

            this.FilesList.Add(fileInfo);
        }

        #endregion

        #region Private Methods - Overrides

        /// <summary>
        /// Gets a friendly description of the <see cref="DirectoryBrowsingResults"/> instance
        /// indicating the root folder path and the folders and files collections sizes.
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            String directorySuffix = null;
            String fileSuffix = null;
            String rootFolder = null;

            if (this.Directories.Count > 1)
                directorySuffix = "s";

            if (this.Files.Count > 1)
                fileSuffix = "s";

            if (! String.IsNullOrEmpty(this.RootFolder))
            {
                rootFolder = String.Format(" ({0})", this.RootFolder);
            }

            return String.Format("{0} folder{1}, {2} file{3}{4}.", 
                this.Directories.Count, directorySuffix, this.Files.Count, fileSuffix, rootFolder);
        }

        #endregion

    }

}
