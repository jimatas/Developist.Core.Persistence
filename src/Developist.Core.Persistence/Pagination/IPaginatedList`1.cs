using System.Collections.Generic;

namespace Developist.Core.Persistence.Pagination
{
    /// <summary>
    /// Represents a paginated list of data.
    /// </summary>
    /// <typeparam name="T">The type of data being paginated.</typeparam>
    public interface IPaginatedList<out T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Gets the current page number.
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
        /// Gets the total number of items.
        /// </summary>
        int ItemCount { get; }
    }
}
