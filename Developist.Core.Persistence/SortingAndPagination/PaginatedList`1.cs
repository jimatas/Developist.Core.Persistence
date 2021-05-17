// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Default implementation of the <see cref="IPaginatedList{T}"/> interface.
    /// </summary>
    /// <typeparam name="T">The type of the list items.</typeparam>
    public class PaginatedList<T> : IPaginatedList<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class given the current page of results and the paginator by which it was created.
        /// </summary>
        /// <param name="currentPage">An enumerable collection with the current page of results.</param>
        /// <param name="paginator">The instructions by which the result set is being paginated over.</param>
        public PaginatedList(IEnumerable<T> currentPage, SortingPaginator<T> paginator)
        {
            if (currentPage is not IList<T> list)
            {
                list = Ensure.Argument.NotNull(currentPage, nameof(currentPage)).ToList();
            }
            InnerList = list;
            Paginator = Ensure.Argument.NotNull(paginator, nameof(paginator));
        }

        /// <summary>
        /// The current page of items in the result set.
        /// </summary>
        protected IList<T> InnerList { get; }

        /// <summary>
        /// The sorting and pagination instructions whereby the result set is being paginated over.
        /// </summary>
        protected SortingPaginator<T> Paginator { get; }

        /// <inheritdoc/>
        public int PageNumber => Paginator.PageNumber;
        /// <inheritdoc/>
        public int PageSize => Paginator.PageSize;
        /// <inheritdoc/>
        public int PageCount => Paginator.PageCount;
        /// <inheritdoc/>
        public int ItemCount => Paginator.ItemCount;

        #region IReadOnlyList<T> members
        /// <inheritdoc/>
        public T this[int index] => InnerList[index];
        /// <inheritdoc/>
        public int Count => InnerList.Count;
        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
