// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

namespace System.Extensions.IO
{

    /// <summary>
    /// The combination of <see cref="BrowserMode"/> and <see cref="DirectoryMatchRules"/>
    /// specify wether child folders should also be searched or not, and how to search them.
    /// Child folders can be excluded or included idependently  using one or more
    /// <see cref="ISearchFilter{T}"/> objects, and they can be searched  to return all of their
    /// contents or just filtered contents when they match themselves directory search criterias.
    /// In short, this class defines introspection techniques that orchestrate search filters.
    /// </summary>
    public sealed class DirectoryBrowserSearchBehavior
    {

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes a new <see cref="DirectoryBrowserSearchBehavior"/> object using the
        /// <see cref="DriveAnalyzer.IO.DirectoryBrowserModes.ExcludeChildDirectories"/> and
        /// <see cref="DriveAnalyzer.IO.DirectoryMatchRules.IncludeMatchingDirectoryContents"/> default values.
        /// </summary>
        public DirectoryBrowserSearchBehavior()
            : this(DirectoryBrowserModes.ExcludeChildDirectories, DirectoryMatchRules.IncludeMatchingDirectoryContents)
        {
        }

        /// <summary>
        /// Initializes a new <see cref="DirectoryBrowserSearchBehavior"/> object using the
        /// provided <see cref="DriveAnalyzer.IO.DirectoryBrowserModes"/> and
        /// <see cref="DriveAnalyzer.IO.DirectoryMatchRules"/> values.
        /// </summary>
        /// <param name="browserMode">
        /// Defines how a <see cref="IDirectoryBrowser"/> object should process
        /// child folders.
        /// </param>
        /// <param name="directoryMatchRules">
        /// When the <see cref="DirectoryBrowserModes"/> is <u>not</u> set to 
        /// <see cref="DirectoryBrowserModes.ExcludeChildDirectories"/>, this value
        /// defines how a <see cref="IDirectoryBrowser"/> object should process
        /// child folders that were identified as positively matching the <see cref="DirectoryBrowserSearchFilters"/>.
        /// </param>
        public DirectoryBrowserSearchBehavior(DirectoryBrowserModes browserMode, DirectoryMatchRules directoryMatchRules)
        {
            this.BrowserMode = browserMode;
            this.DirectoryMatchRules = directoryMatchRules;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Defines how a <see cref="IDirectoryBrowser"/> object should process
        /// child folders.
        /// </summary>
        public DirectoryBrowserModes BrowserMode
        { get; set; }

        /// <summary>
        /// When the <see cref="DirectoryBrowserModes"/> is <u>not</u> set to 
        /// <see cref="DirectoryBrowserModes.ExcludeChildDirectories"/>, this value
        /// defines how a <see cref="IDirectoryBrowser"/> object should process
        /// child folders that were identified as positively matching the <see cref="DirectoryBrowserSearchFilters"/>.
        /// </summary>
        public DirectoryMatchRules DirectoryMatchRules
        { get; set; }

        #endregion

    }

}
