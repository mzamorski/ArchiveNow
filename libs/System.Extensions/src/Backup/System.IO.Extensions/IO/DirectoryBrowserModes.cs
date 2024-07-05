// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

namespace System.Extensions.IO
{

    /// <summary>
    /// Defines how a <see cref="IDirectoryBrowser"/> object should process
    /// child folders.
    /// </summary>
    public enum DirectoryBrowserModes
    {
        /// <summary>
        /// When this mode is chosen, a <see cref="IDirectoryBrowser"/> will
        /// never scan the contents of its <see cref="IDirectoryBrowser.RootFolder"/> sub-folders.
        /// </summary>
        ExcludeChildDirectories,

        /// <summary>
        /// When this mode is chosen, a <see cref="IDirectoryBrowser"/> will
        /// always scan the contents of sub-folders regardless wether the sub-folders
        /// themselves match the <see cref="DirectoryBrowserSearchFilters"/> or not.
        /// </summary>
        SearchAllChildDirectories,

        /// <summary>
        /// When this mode is chosen, a <see cref="IDirectoryBrowser"/> will
        /// scan the contents of sub-folders only if these sub-folders themselves
        /// match the <see cref="DirectoryBrowserSearchFilters"/>.
        /// </summary>
        SearchOnlyMatchingDirectories,
    }

}
