using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Filtering
{
    /// <summary>
    /// An implementation of the <see cref="IFilterCriteria{TEntity}"/> interface that filters entities using a predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity to filter.</typeparam>
    public class PredicateFilterCriteria<T> : IFilterCriteria<T>
    {
        private readonly Expression<Func<T, bool>> _predicate;

        /// <summary>
        /// Initializes a new instance of the <see cref="PredicateFilterCriteria{TEntity}"/> class with the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to use for filtering entities.</param>
        public PredicateFilterCriteria(Expression<Func<T, bool>> predicate)
        {
            _predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
        }

        /// <summary>
        /// Filters the specified query using the predicate.
        /// </summary>
        /// <param name="query">The query to filter.</param>
        /// <returns>A new query that includes only the entities that match the predicate.</returns>
        public IQueryable<T> Filter(IQueryable<T> query) => query.Where(_predicate);
    }
}
