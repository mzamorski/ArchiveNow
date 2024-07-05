// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//

namespace System.Extensions.IO
{

    /// <summary>
    /// When a <see cref="ISearchFilter{T}"/> defines that a particular object
    /// matches its criterias, this search option defines wether
    /// the object should actually be rejected or be held as a part of the
    /// <see cref="DirectoryBrowsingResults"/>.
    /// </summary>
    public enum SearchOptions
    {
        /// <summary>
        /// Matching items should be held as part of the <see cref="DirectoryBrowsingResults"/>.
        /// </summary>
        IncludeMatches,

        /// <summary>
        /// Unmatching items should be held as part of the <see cref="DirectoryBrowsingResults"/>.
        /// </summary>
        ExcludeMatches,
    }

}
