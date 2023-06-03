using System;
using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IQueryable{T}"/> objects.
    /// </summary>
    internal static partial class QueryableExtensions
    {
        /// <summary>
        /// Filters the specified query using the specified filter criteria.
        /// </summary>
        /// <typeparam name="T">The type of entity being filtered.</typeparam>
        /// <param name="query">The query to filter.</param>
        /// <param name="criteria">The filter criteria to apply to the query.</param>
        /// <returns>The filtered query.</returns>
        public static IQueryable<T> FilterBy<T>(this IQueryable<T> query, IFilterCriteria<T> criteria)
        {
            if (criteria is null)
            {
                throw new ArgumentNullException(nameof(criteria));
            }

            return criteria.Filter(query);
        }
    }
}
