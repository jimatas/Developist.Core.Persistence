namespace Developist.Core.Persistence;

/// <summary>
/// Defines the criteria for pagination including page size, page number, and sorting parameters.
/// </summary>
/// <typeparam name="T">The type of data the pagination criteria applies to.</typeparam>
public interface IPaginationCriteria<T> : ICriteria<T>
{
    /// <summary>
    /// Gets the number of the page to be retrieved.
    /// Page numbering starts at 1.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the number of results per page.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the read-only list of sort criteria to be applied to the paginated data.
    /// </summary>
    /// <remarks>
    /// The use of an <see cref="IReadOnlyList{T}"/> ensures the immutability of the sort criteria list,
    /// while preserving the sequence in which the criteria are to be applied.
    /// </remarks>
    IReadOnlyList<ISortCriterion<T>> SortCriteria { get; }
}
