using Developist.Core.Persistence.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to support pagination.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Asynchronously creates a paginated list from an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to paginate.</param>
    /// <param name="paginationCriteria">The pagination criteria.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list.</returns>
    public static async Task<IPaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query,
        IPaginationCriteria<T> paginationCriteria,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        query = query.Paginate(paginationCriteria);
        var list = await query.ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PaginatedList<T>(paginationCriteria.PageNumber, paginationCriteria.PageSize, list, totalCount);
    }

    /// <summary>
    /// Asynchronously creates a paginated list from an <see cref="IQueryable{T}"/>, using a configuration action to specify the pagination criteria.
    /// </summary>
    /// <typeparam name="T">The type of elements in the query.</typeparam>
    /// <param name="query">The <see cref="IQueryable{T}"/> to paginate.</param>
    /// <param name="configurePagination">The action used to configure the pagination criteria.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the paginated list.</returns>
    public static Task<IPaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query,
        Action<PaginationCriteriaConfigurator<T>> configurePagination,
        CancellationToken cancellationToken = default)
    {
        var paginationCriteria = new PaginationCriteria<T>();
        configurePagination(new PaginationCriteriaConfigurator<T>(paginationCriteria));

        return query.ToPaginatedListAsync(paginationCriteria, cancellationToken);
    }
}
