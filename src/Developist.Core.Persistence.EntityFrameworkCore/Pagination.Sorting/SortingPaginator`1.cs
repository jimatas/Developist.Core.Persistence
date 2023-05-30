using Developist.Core.Persistence.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore.Pagination.Sorting;

/// <summary>
/// An Entity Framework Core paginator that sorts and paginates data.
/// </summary>
/// <typeparam name="T">The type of items being paged.</typeparam>
public class SortingPaginator<T> : Persistence.Pagination.Sorting.SortingPaginator<T>
{
    /// <inheritdoc/>
    public override async Task<IPaginatedList<T>> PaginateAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        ItemCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<T> innerList = await PaginateCore(query).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PaginatedList<T>(innerList, PageNumber, PageSize, ItemCount);
    }
}
