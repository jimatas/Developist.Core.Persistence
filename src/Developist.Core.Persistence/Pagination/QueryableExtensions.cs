using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to enable pagination.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Paginates the query according to the specified pagination criteria.
    /// </summary>
    /// <typeparam name="T">The type of the items in the query.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="paginationCriteria">The pagination criteria to apply.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the paginated query.</returns>
    public static IQueryable<T> Paginate<T>(this IQueryable<T> query, IPaginationCriteria<T> paginationCriteria)
    {
        Ensure.NotNull(paginationCriteria);

        return paginationCriteria.Apply(query);
    }
}
