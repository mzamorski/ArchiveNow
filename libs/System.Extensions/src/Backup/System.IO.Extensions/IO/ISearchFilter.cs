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
    /// Defines generic members for search filters to be used with
    /// <see cref="IDirectoryBrowser"/> objects.
    /// </summary>
    /// <typeparam name="T">
    /// The type of objects that will be passed to the <see cref="Matches"/>
    /// method.
    /// </typeparam>
    public interface ISearchFilter<T>
    {

        #region Properties

        /// <summary>
        /// Specifies wether matches should be held as parts of the results, or rejected.
        /// </summary>
        SearchOptions SearchOption
        { get; set; }

        /// <summary>
        /// Returns wether the <see cref="ISearchFilter{T}"/> actually contains criterias.
        /// </summary>
        Boolean ContainsFilterCriterias
        { get; }

        #endregion

        #region Methods

        /// <summary>
        /// Returns wether the object of type <see cref="T"/> matches,
        /// or doesn't match, depending on the value of the <see cref="SearchOption"/> property
        /// and according to the filter options defined in this search filter.
        /// </summary>
        /// <param name="item">The object that must be analyzed.</param>
        /// <returns></returns>
        Boolean Matches(T item);

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
        Boolean Matches(T item, ExceptionHandler errorHandler);

        #endregion

    }

}
