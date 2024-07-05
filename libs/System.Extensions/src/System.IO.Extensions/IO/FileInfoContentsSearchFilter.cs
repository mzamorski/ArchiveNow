// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System;
using System.IO;

namespace System.Extensions.IO
{

    /// <summary>
    /// Basic search filter that can read a file and return wether it contains
    /// a particular string. This search filter is really basic and is present only
    /// to illustrate a concept. It is neither performant, neither safe, neither powerful
    /// and it does not even perform case-insensitive searches.
    /// </summary>
    public class FileInfoContentsSearchFilter
        : SearchFilter<FileInfo>
    {

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes a new instance without specifying any of the options
        /// defining how to test the match.
        /// </summary>
        public FileInfoContentsSearchFilter()
            : this(null, SearchOptions.IncludeMatches, null)
        {
        }

        /// <summary>
        /// Initializes a new instance without specifying any of the options
        /// defining how to test the match, and decorates the specified
        /// <see cref="ISearchFilter{T}"/>.
        /// </summary>
        /// <param name="innerFilter">
        /// The <see cref="ISearchFilter{T}"/> that should be decorated.
        /// </param>
        public FileInfoContentsSearchFilter(ISearchFilter<FileInfo> innerFilter)
            : this(innerFilter, SearchOptions.IncludeMatches, null)
        {
        }

        /// <summary>
        /// Initializes a new instance with the required options for defining how to
        /// test the match.
        /// </summary>
        /// <param name="searchOption">
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="textToFind">The text to find in the file.</param>
        public FileInfoContentsSearchFilter(SearchOptions searchOption, String textToFind)
            : this(null, searchOption, textToFind)
        {
        }

        /// <summary>
        /// Initializes a new instance without specifying the <see cref="FileAttributes"/>
        /// and decorates the specified <see cref="ISearchFilter{T}"/>.
        /// </summary>
        /// <param name="innerFilter">
        /// The <see cref="ISearchFilter{T}"/> that should be decorated.
        /// </param>
        /// <param name="searchOption">
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="textToFind">The text to find in the file.</param>
        public FileInfoContentsSearchFilter(ISearchFilter<FileInfo> innerFilter, SearchOptions searchOption, String textToFind)
            : base(innerFilter)
        {
            this.SearchOption = searchOption;
            this.TextToFind = textToFind;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// The text to find in the <see cref="FileInfo"/> objects.
        /// </summary>
        public String TextToFind
        { get; set; }

        /// <summary>
        /// Returns wether the <see cref="TextToFind"/> property value
        /// contains characters.
        /// </summary>
        public override Boolean ContainsFilterCriterias
        {
            get
            {
                return (
                    base.ContainsFilterCriterias ||
                    (!String.IsNullOrEmpty(this.TextToFind))
                    );
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// By default, returns wether the <see cref="FileSystemInfo"/> object contents,
        /// or doesn't content, the text to find, depending on the value of the
        /// <see cref="SearchOption"/> property.
        /// </summary>
        /// <param name="item">
        /// The <see cref="FileSystemInfo"/> object to validate against the provided
        /// text to find.
        /// </param>
        /// <returns></returns>
        protected override sealed Boolean IsMatch(FileInfo item)
        {
            if (item == null || !File.Exists(item.FullName))
                return false;

            if (!this.ContainsFilterCriterias)
                return true;

            String fileContents = File.ReadAllText(item.FullName);

            Boolean result = !String.IsNullOrEmpty(fileContents) && fileContents.Contains(this.TextToFind);

            return (this.SearchOption == SearchOptions.IncludeMatches ? result : !result);
        }

        #endregion

    }

}

