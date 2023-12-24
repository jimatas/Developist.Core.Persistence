namespace Developist.Core.Persistence;

/// <summary>
/// Represents a read-only list of items of type <typeparamref name="T"/> that is paginated.
/// </summary>
/// <typeparam name="T">The type of the items in the paginated list.</typeparam>
public interface IPaginatedList<out T> : IReadOnlyList<T>
{
    /// <summary>
    /// Gets the current page number, starting from 1 for the first page.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the number of items per page.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the total number of pages.
    /// </summary>
    int PageCount { get; }

    /// <summary>
    /// Gets the total number of items across all pages.
    /// </summary>
    int TotalCount { get; }
}
