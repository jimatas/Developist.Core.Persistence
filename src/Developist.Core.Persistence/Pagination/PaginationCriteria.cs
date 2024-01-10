using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence;

/// <summary>
/// Default implementation of the <see cref="IPaginationCriteria{T}"/> interface that supports sorting by multiple properties.
/// </summary>
/// <typeparam name="T">The type of the items being paginated.</typeparam>
public class PaginationCriteria<T> : IPaginationCriteria<T>
{
    /// <summary>
    /// The default size of a page.
    /// </summary>
    public const int DefaultPageSize = 25;

    private int _pageNumber;
    private int _pageSize;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationCriteria{T}"/> class with the default page number and page size.
    /// </summary>
    public PaginationCriteria()
        : this(pageNumber: 1) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationCriteria{T}"/> class with the specified page number and the default page size.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    public PaginationCriteria(int pageNumber)
        : this(pageNumber, PaginationCriteria<T>.DefaultPageSize) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationCriteria{T}"/> class with the specified page number and page size.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items to include in each page.</param>
    public PaginationCriteria(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    /// Gets or sets the number of the page to be retrieved.
    /// Page numbering starts at 1.
    /// </summary>
    public int PageNumber
    {
        get => _pageNumber;
        set => _pageNumber = Ensure.Argument.NotOutOfRange(value, minValue: 1, paramName: nameof(PageNumber));
    }

    /// <summary>
    /// Gets or sets the number of results per page.
    /// </summary>
    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = Ensure.Argument.NotOutOfRange(value, minValue: 1, paramName: nameof(PageSize));
    }

    /// <summary>
    /// Gets a list of <see cref="ISortCriterion{T}"/> objects representing the properties by which the results are sorted.
    /// </summary>
    public IList<ISortCriterion<T>> SortCriteria { get; } = new List<ISortCriterion<T>>();

    /// <inheritdoc/>
    IReadOnlyList<ISortCriterion<T>> IPaginationCriteria<T>.SortCriteria => SortCriteria.ToArray();

    /// <inheritdoc/>
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        Ensure.Argument.NotNull(query);

        query = ApplySorting(query);
        query = ApplyPagination(query);

        return query;
    }

    private IQueryable<T> ApplySorting(IQueryable<T> query)
    {
        IOrderedQueryable<T>? sortedQuery = null;

        foreach (var criterion in SortCriteria)
        {
            sortedQuery = sortedQuery is null
                ? criterion.Apply(query)
                : criterion.Apply(sortedQuery);
        }

        return sortedQuery ?? query;
    }

    private IQueryable<T> ApplyPagination(IQueryable<T> query)
    {
        return query.Skip((PageNumber - 1) * PageSize).Take(PageSize);
    }
}
