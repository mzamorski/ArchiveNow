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
    /// Implements <see cref="ISearchFilter{T}"/> on
    /// <see cref="FileSystemInfo"/> objects (<see cref="FileInfo"/>,
    /// <see cref="DirectoryInfo"/>) and returns wether a given
    /// folder or file as a given attribute. Multiple attributes can be
    /// combined but it is more restrictive than using multiple
    /// <see cref="FileSystemInfoAttributeSearchFilter{T}"/>, each
    /// specifying only one attribute. Multiple filters can be stacked thanks
    /// to the fact that the <see cref="SearchFilter{T}"/> base class implements
    /// a Decorator pattern.
    /// </summary>
    public class FileSystemInfoAttributeSearchFilter<T>
        : SearchFilter<T>
        where T : FileSystemInfo
    {

        #region Private Fields

        private Boolean _containsFilterCriterias;
        private FileAttributes _attributes;

        #endregion

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes a new instance without specifying any of the options
        /// defining how to test the match.
        /// </summary>
        public FileSystemInfoAttributeSearchFilter()
            : this(null)
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
        public FileSystemInfoAttributeSearchFilter(ISearchFilter<T> innerFilter)
            : this(innerFilter, SearchOptions.IncludeMatches)
        {
        }

        /// <summary>
        /// Initializes a new instance with the required options for defining how to
        /// test the match.
        /// </summary>
        /// <param name="searchOption">
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="attributes">
        /// The <see cref="FileAttributes"/> that <see cref="FileSystemInfo"/> objects should have.
        /// </param>
        public FileSystemInfoAttributeSearchFilter(SearchOptions searchOption, FileAttributes attributes)
            : this(null, searchOption, attributes)
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
        public FileSystemInfoAttributeSearchFilter(ISearchFilter<T> innerFilter, SearchOptions searchOption)
            : base(innerFilter)
        {
            this.SearchOption = searchOption;
        }

        /// <summary>
        /// Initializes a new instance with the required options for defining how to
        /// test the match and decorates the specified <see cref="ISearchFilter{T}"/>.
        /// </summary>
        /// <param name="innerFilter">
        /// The <see cref="ISearchFilter{T}"/> that should be decorated.
        /// </param>
        /// <param name="searchOption">
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </param>
        /// <param name="attributes">
        /// The <see cref="FileAttributes"/> that <see cref="FileSystemInfo"/> objects should have.
        /// </param>
        public FileSystemInfoAttributeSearchFilter(ISearchFilter<T> innerFilter, SearchOptions searchOption, FileAttributes attributes)
            : base(innerFilter)
        {
            this.SearchOption = searchOption;
            this.Attributes = attributes;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns wether FileAttributes were provided, either during instantiation
        /// as constructor arguments or using the <see cref="Attributes"/> property
        /// setter.
        /// </summary>
        public override Boolean ContainsFilterCriterias
        {
            get
            {
                return (
                    base.ContainsFilterCriterias ||
                    this._containsFilterCriterias
                    );
                
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="FileAttributes"/> that must used to determine wether
        /// <see cref="FileSystemInfo"/> objects match.
        /// </summary>
        public FileAttributes Attributes
        {
            get { return this._attributes; }
            set
            {
                this._containsFilterCriterias = true;
                this._attributes = value;
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Returns wether the <see cref="FileSystemInfo"/> object has,
        /// or hasn't, the specified <see cref="FileAttributes"/>, depending
        /// on the value of the <see cref="SearchOption"/> property.
        /// </summary>
        /// <param name="item">
        /// The <see cref="FileSystemInfo"/> object to validate against the
        /// provided <see cref="FileAttributes"/>.
        /// </param>
        /// <returns></returns>
        protected override sealed Boolean IsMatch(T item)
        {
            if (item == null)
                return false;

            if (!this.ContainsFilterCriterias)
                return true;

            // Sadly enough, the line that performs the Attributes verification also returns TRUE
            // when the folder or file does not exist. We therefore first ensure that the object exists,
            // but we do not use the FileSystemInfo.Exists() method because it is not reliable, sometimes
            // it does not return correct values.
            if (!File.Exists(item.FullName) && !Directory.Exists(item.FullName))
                return false;

            Boolean result = ((item.Attributes & this.Attributes) == this.Attributes);

            return (this.SearchOption == SearchOptions.IncludeMatches ? result : !result);
        }

        #endregion

    }

}

