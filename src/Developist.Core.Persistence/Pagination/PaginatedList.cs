using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Represents a generic paginated list of items.
/// </summary>
/// <typeparam name="T">The type of the items in the list.</typeparam>
public class PaginatedList<T> : IPaginatedList<T>
{
    /// <summary>
    /// An empty instance of the <see cref="PaginatedList{T}"/> class.
    /// </summary>
    public static readonly PaginatedList<T> Empty = new(
        pageNumber: 1,
        pageSize: PaginationCriteria<T>.DefaultPageSize,
        innerList: Array.Empty<T>(),
        totalCount: 0);

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
    /// </summary>
    /// <param name="pageNumber">The current page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="innerList">The list of items corresponding to the current page of results.</param>
    /// <param name="totalCount">The total number of items in the list.</param>
    public PaginatedList(int pageNumber, int pageSize, IReadOnlyList<T> innerList, int totalCount)
    {
        PageNumber = Ensure.NotLessThan(1, pageNumber);
        PageSize = Ensure.NotLessThan(1, pageSize);
        PageCount = CalculatePageCount(totalCount, pageSize);
        InnerList = Ensure.NotNull(innerList);
        TotalCount = Ensure.NotLessThan(innerList.Count, totalCount);
    }

    /// <inheritdoc/>
    public int PageNumber { get; }

    /// <inheritdoc/>
    public int PageSize { get; }

    /// <inheritdoc/>
    public int PageCount { get; }

    /// <inheritdoc/>
    public int TotalCount { get; }

    /// <inheritdoc/>
    public int Count => InnerList.Count;

    /// <inheritdoc/>
    public T this[int index] => InnerList[index];

    /// <summary>
    /// Provides access to the list of items corresponding to the current page of results.
    /// </summary>
    protected IReadOnlyList<T> InnerList { get; }

    /// <inheritdoc/>
    public IEnumerator<T> GetEnumerator()
    {
        return InnerList.GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    private static int CalculatePageCount(int totalCount, int pageSize)
    {
        return (int)Math.Ceiling((double)totalCount / pageSize);
    }
}
