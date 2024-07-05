// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Extensions.Text;

namespace System.Extensions.IO
{

    /// <summary>
    /// Implements <see cref="ISearchFilter{T}"/> on
    /// <see cref="FileSystemInfo"/> objects (<see cref="FileInfo"/>,
    /// <see cref="DirectoryInfo"/>) and returns wether a given
    /// folder or file name matches a wildcard or Regular Expression. Multiple wildcard
    /// patterns or Regular Expressions can be specified. It is possible to specify
    /// multiple patterns for exclusion and multiple patterns for inclusion by combining
    /// multiple filters. Indeed, multiple filters can be stacked thanks to the fact that
    /// the <see cref="SearchFilter{T}"/> base class implements a Decorator pattern.
    /// </summary>
    public class FileSystemInfoNameSearchFilter<T>
        : SearchFilter<T>
        where T: FileSystemInfo
    {

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes a new instance without specifying any of the options
        /// defining how to test the match.
        /// </summary>
        public FileSystemInfoNameSearchFilter()
            : this(null, SearchOptions.IncludeMatches, new List<String>())
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
        public FileSystemInfoNameSearchFilter(ISearchFilter<T> innerFilter)
            : this(innerFilter, SearchOptions.IncludeMatches, new List<String>())
        {
        }

        /// <summary>
        /// Initializes a new instance with the required options for defining how to
        /// test the match.
        /// </summary>
        /// <param name="searchOption">
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="patterns">
        /// The list of wildcards or Regular Expressions that can be used to check for a match.
        /// </param>
        public FileSystemInfoNameSearchFilter(SearchOptions searchOption, ICollection<String> patterns)
            : this(null, searchOption, patterns)
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
        /// <param name="patterns">
        /// The list of wildcards or Regular Expressions that can be used to check for a match.
        /// </param>
        public FileSystemInfoNameSearchFilter(ISearchFilter<T> innerFilter, SearchOptions searchOption, ICollection<String> patterns)
            : base(innerFilter)
        {
            this.SearchOption = searchOption;
            this.Patterns = patterns;
            this.MatchMode = FileSystemInfoNameMatchMode.UseWildcards;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of wildcards or Regular Expressions that are used to
        /// check for a match.
        /// </summary>
        public ICollection<String> Patterns
        { get; private set; }

        /// <summary>
        /// Returns wether the <see cref="Patterns"/> list contains at least
        /// one wildcard or Regular Expression.
        /// </summary>
        public override Boolean ContainsFilterCriterias
        {
            get
            {
                return (
                    base.ContainsFilterCriterias ||
                    (this.Patterns != null && this.Patterns.Count != 0)
                    );
            }
        }

        /// <summary>
        /// Defines wether the <see cref="Patterns"/> should be treated as
        /// wildcards or as Regular Expressions during the match verification
        /// process.
        /// </summary>
        public FileSystemInfoNameMatchMode MatchMode
        { get; set; }

        #endregion

        #region Protected Methods

        /// <summary>
        /// By default, returns wether the <see cref="FileSystemInfo"/> object has,
        /// or hasn't, a name matching at least one of the wildcard patterns or Regular
        /// Expressions, depending on the value of the <see cref="SearchOption"/> property.
        /// It is possible to request a match verification based on Regular Expressions by
        /// setting the appropriate value for the <see cref="MatchMode"/> property.
        /// </summary>
        /// <param name="item">
        /// The <see cref="FileSystemInfo"/> object to validate against the provided
        /// wildcard patterns or Regular Expressions.
        /// </param>
        /// <returns></returns>
        protected override sealed Boolean IsMatch(T item)
        {
            Boolean result = false;
            String text = item == null ? null : item.Name;

            if (item == null || String.IsNullOrEmpty(text))
                return false;

            if (! this.ContainsFilterCriterias)
                return true;

            foreach (String pattern in this.Patterns)
            {
                if (this.CreateMatchObject(pattern).IsMatch(text))
                {
                    result = true;
                    break;
                }
            }

            return (this.SearchOption == SearchOptions.IncludeMatches ? result : !result);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Depending on the value of the <see cref="MatchMode"/> property,
        /// returns a <see cref="Regex"/> or a <see cref="Wildcard"/> object to
        /// perform a match verification.
        /// </summary>
        /// <param name="pattern">
        /// The String pattern to use during the match verification process. Both
        /// wildcards and Regular Expressions are possible.
        /// </param>
        /// <returns></returns>
        private Regex CreateMatchObject(String pattern)
        {
            if (this.MatchMode == FileSystemInfoNameMatchMode.UseRegEx)
                return new Regex(pattern, RegexOptions.IgnoreCase);

            return new Wildcard(pattern, RegexOptions.IgnoreCase);
        }

        #endregion

    }

}

