// ------------------------------------------------------------------------------------------------------------------//
// Developed by: Jean-Christophe Grégoire
// CodeProject profile: http://www.codeproject.com/script/Membership/Profiles.aspx?mid=496750
// e-Mail address: dio@dioland.com
// Initial Release Date: 01/01/2009
// ------------------------------------------------------------------------------------------------------------------//
using System;

namespace System.Extensions.IO
{

    /// <summary>
    /// This base class implements a Decorator pattern on top of
    /// the <see cref="ISearchFilter{T}"/> interface. It makes it
    /// possible to "stack" multiple implementations of the
    /// <see cref="ISearchFilter{T}"/> interface and combine their
    /// results. This makes virtually any kind of search possible.
    /// </summary>
    public abstract class SearchFilter<T>
        : ISearchFilter<T>
    {

        #region Constructor(s) / Destructor

        /// <summary>
        /// Initializes the new instance.
        /// </summary>
        protected SearchFilter()
            : this(null)
        {
        }

        /// <summary>
        /// Initializes the new instance and decorates another existing
        /// instance.
        /// </summary>
        /// <param name="innerFilter">
        /// The <see cref="ISearchFilter{T}"/> that should be decorated.
        /// </param>
        protected SearchFilter(ISearchFilter<T> innerFilter)
        {
            this.InnerFilter = innerFilter;
        }

        #endregion

        #region Protected Properties

        /// <summary>
        /// Returns wether the current instance is decorating another
        /// <see cref="ISearchFilter{T}"/> instance. In other words,
        /// returns wether <see cref="InnerFilter"/> is not null.
        /// </summary>
        protected Boolean HasInnerFilter
        {get { return this.InnerFilter != null; }}

        /// <summary>
        /// An optional reference to another "decorated" instance.
        /// </summary>
        protected ISearchFilter<T> InnerFilter
        { get; set; }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns wether the <see cref="InnerFilter"/> property references another
        /// existing <see cref="ISearchFilter{T}"/> that has criterias.
        /// </summary>
        public virtual Boolean ContainsFilterCriterias
        {
            get
            {
                return (this.InnerFilter != null && this.InnerFilter.ContainsFilterCriterias);
            }  
        }

        #endregion

        #region ISearchFilter

        /// <summary>
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </summary>
        public SearchOptions SearchOption
        { get; set; }

        /// <summary>
        /// Returns wether the object of type <see cref="T"/> matches,
        /// or doesn't match, depending on the value of the <see cref="SearchOption"/> property
        /// and according to the filter options defined in this search filter. If an existing decorated
        /// <see cref="InnerFilter"/> object returns a negative match, the method will in turn also
        /// return a negative match and will therefore skip its own match verification.
        /// </summary>
        /// <param name="item">The object that must be analyzed.</param>
        /// <returns></returns>
        public Boolean Matches(T item)
        {
            return this.Matches(item, null);
        }

        /// <summary>
        /// Returns wether the object of type <see cref="T"/> matches,
        /// or doesn't match, depending on the value of the <see cref="SearchOption"/> property
        /// and according to the filter options defined in this search filter.
        /// </summary>
        /// <param name="item">The object that must be analyzed.</param>
        /// <param name="errorHandler">
        /// An optional delegate to handle eventual errors. When no delegate is
        /// specified, the search filter silently swallows the exception and returns a
        /// "no match".
        /// </param>
        /// <returns></returns>
        public Boolean Matches(T item, ExceptionHandler errorHandler)
        {

            if (this.HasInnerFilter && !this.InnerFilter.Matches(item, errorHandler))
            {
                return false;
            }

            try
            {
                return this.IsMatch(item);
            }
            catch (Exception ex)
            {
                if (errorHandler != null)
                {
                    errorHandler(ex);
                }
                return false;
            }

        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Executes the actual job of verifying if there is a match for the concrete implementation
        /// that inherits from this base class.
        /// </summary>
        /// <param name="item">The object that must be analyzed.</param>
        /// <returns></returns>
        protected abstract Boolean IsMatch(T item);

        #endregion

    }

}
