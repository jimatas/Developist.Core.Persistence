using System;
using System.Collections;
using System.Collections.Generic;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Represents a generic paginated list of items.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list.</typeparam>
    public class PaginatedList<T> : IPaginatedList<T>
    {
        /// <summary>
        /// An empty instance of the <see cref="PaginatedList{T}"/> class.
        /// </summary>
        public static readonly PaginatedList<T> Empty = new PaginatedList<T>(Array.Empty<T>(), pageNumber: 1, pageSize: 1, itemCount: 0);

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
        /// </summary>
        /// <param name="innerList">The list of items corresponding to the current page of results.</param>
        /// <param name="pageNumber">The current page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <param name="itemCount">The total number of items in the list.</param>
        public PaginatedList(IReadOnlyList<T> innerList, int pageNumber, int pageSize, int itemCount)
        {
            InnerList = innerList ?? throw new ArgumentNullException(nameof(innerList));
            PageNumber = pageNumber >= 1 ? pageNumber
                : throw new ArgumentOutOfRangeException(
                    paramName: nameof(pageNumber),
                    actualValue: pageNumber,
                    message: "Value cannot be less than 1.");

            PageSize = pageSize >= 1 ? pageSize
                : throw new ArgumentOutOfRangeException(
                    paramName: nameof(pageSize),
                    actualValue: pageSize,
                    message: "Value cannot be less than 1.");

            PageCount = (int)Math.Ceiling((double)itemCount / pageSize);
            ItemCount = itemCount >= innerList.Count ? itemCount
                : throw new ArgumentOutOfRangeException(
                    paramName: nameof(itemCount),
                    actualValue: itemCount,
                    message: $"Value cannot be less than {innerList.Count}.");
        }

        /// <summary>
        /// Provides access to the list of items corresponding to the current page of results.
        /// </summary>
        protected IReadOnlyList<T> InnerList { get; }

        /// <inheritdoc/>
        public int PageNumber { get; }

        /// <inheritdoc/>
        public int PageSize { get; }

        /// <inheritdoc/>
        public int PageCount { get; }

        /// <inheritdoc/>
        public int ItemCount { get; }

        /// <inheritdoc/>
        public int Count => InnerList.Count;

        /// <inheritdoc/>
        public T this[int index] => InnerList[index];

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
