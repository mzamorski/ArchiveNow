// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

namespace System.Extensions.IO
{

    /// <summary>
    /// When the <see cref="DirectoryBrowserModes"/> is <u>not</u> set to 
    /// <see cref="DirectoryBrowserModes.ExcludeChildDirectories"/>, this enumerator
    /// defines how a <see cref="IDirectoryBrowser"/> object should process
    /// child folders that were identified as positively matching the <see cref="DirectoryBrowserSearchFilters"/>.
    /// When a mix of matching and unmatching folders is found, unmatching folders
    /// are searched normally and separately.
    /// </summary>
    public enum DirectoryMatchRules
    {
        /// <summary>
        /// Defines that the complete folders and files hierarchy below the
        /// <see cref="IDirectoryBrowser.RootFolder"/> should be added to the
        /// <see cref="DirectoryBrowsingResults"/> without making use of the
        /// <see cref="DirectoryBrowserSearchFilters"/>. This can be useful in a
        /// mass-delete scenario.
        /// </summary>
        /// <example>
        /// Think for example of deleting all Subversion ".svn" folders with their
        /// contents, and then also a couple of additional targets like all "*.bak"
        /// files in other folders.
        /// </example>
        IncludeCompleteDirectoryContents,

        /// <summary>
        /// Defines that the complete folders and files hierarchy below the
        /// <see cref="IDirectoryBrowser.RootFolder"/> should be further
        /// inspected for folders and files matching the <see cref="DirectoryBrowserSearchFilters"/>.
        /// Folders are generally not pretty much interesting in this scenario, it is really
        /// about finding specific files - an maybe folders - matching specific search filters.
        /// </summary>
        IncludeMatchingDirectoryContents,
    }

}
