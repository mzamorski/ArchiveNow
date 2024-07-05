// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System.IO;

namespace System.Extensions.IO
{

    /// <summary>
    /// 
    /// </summary>
    public interface IDirectoryBrowserSearchFilters
    {

        #region Properties

        /// <summary>
        /// A <see cref="ISearchFilter{DirectoryInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a folder should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{DirectoryInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </summary>
        ISearchFilter<DirectoryInfo> DirectoryFilter
        { get; set; }

        /// <summary>
        /// A <see cref="ISearchFilter{FileInfo}"/> object to be used by the
        /// <see cref="IDirectoryBrowser"/> object while testing wether a file should be
        /// added to the results. You are not restricted to the use of one single search filter, stacking
        /// multiple <see cref="ISearchFilter{FileInfo}"/> filter objects  for folders and/or files is possible
        /// when using a type deriving from <see cref="SearchFilter{T}"/> because it implements
        /// a Decorator pattern.
        /// </summary>
        ISearchFilter<FileInfo> FileFilter
        { get; set; }

        #endregion

    }

}
