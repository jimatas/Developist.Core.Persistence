namespace Developist.Core.Persistence;

/// <summary>
/// Provides extension methods for determining the availability of next and previous pages in an <see cref="IPaginatedList{T}"/>.
/// </summary>
public static class PaginatedListExtensions
{
    /// <summary>
    /// Determines whether the paginated list has a next page.
    /// </summary>
    /// <param name="paginatedList">The paginated list to check.</param>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    /// <returns><see langword="true"/> if the paginated list has a next page; otherwise, <see langword="false"/>.</returns>
    public static bool HasNextPage<T>(this IPaginatedList<T> paginatedList)
    {
        return paginatedList.PageNumber < paginatedList.PageCount;
    }

    /// <summary>
    /// Determines whether the paginated list has a previous page.
    /// </summary>
    /// <param name="paginatedList">The paginated list to check.</param>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    /// <returns><see langword="true"/> if the paginated list has a previous page; otherwise, <see langword="false"/>.</returns>
    public static bool HasPreviousPage<T>(this IPaginatedList<T> paginatedList)
    {
        return paginatedList.PageNumber > 1;
    }
}
