using System.Linq;

namespace Developist.Core.Persistence.Filtering
{
    /// <summary>
    /// Represents criteria for filtering entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public interface IFilterCriteria<T>
    {
        /// <summary>
        /// Filters the given <paramref name="query"/> to produce a new <see cref="IQueryable{T}"/> that satisfies the filter criteria.
        /// </summary>
        /// <param name="query">The <see cref="IQueryable{T}"/> to filter.</param>
        /// <returns>A new <see cref="IQueryable{T}"/> that satisfies the filter criteria.</returns>
        IQueryable<T> Filter(IQueryable<T> query);
    }
}
