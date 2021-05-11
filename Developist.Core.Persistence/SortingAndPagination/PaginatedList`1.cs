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
        public PaginatedList(IEnumerable<T> currentPage, SortingPaginator<T> paginator)
        {
            if (currentPage is not IList<T> list)
            {
                list = Ensure.Argument.NotNull(currentPage, nameof(currentPage)).ToList();
            }
            InnerList = list;
            Paginator = Ensure.Argument.NotNull(paginator, nameof(paginator));
        }

        protected IList<T> InnerList { get; }
        protected SortingPaginator<T> Paginator { get; }

        public int PageNumber => Paginator.PageNumber;
        public int PageSize => Paginator.PageSize;
        public int PageCount => Paginator.PageCount;
        public int ItemCount => Paginator.ItemCount;

        #region IReadOnlyList<T> members
        public T this[int index] => InnerList[index];
        public int Count => InnerList.Count;
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        #endregion
    }
}
