using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// An Entity Framework Core paginator that asynchronously sorts and paginates data.
/// </summary>
/// <typeparam name="T">The type of items being paged.</typeparam>
public class SortingPaginator<T> : Persistence.SortingPaginator<T>
{
    /// <inheritdoc/>
    public override async Task<IPaginatedList<T>> PaginateAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
    {
        ItemCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

        IReadOnlyList<T> innerList = await PaginateCore(query).ToListAsync(cancellationToken).ConfigureAwait(false);

        return new PaginatedList<T>(innerList, PageNumber, PageSize, ItemCount);
    }
}
