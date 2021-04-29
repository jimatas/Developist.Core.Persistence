// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Samples
{
    public class PaginatedList<T> : IPaginatedList<T>
    {
        public PaginatedList(IEnumerable<T> result, SortingPaginator<T> paginator)
        {
            if (result is not IList<T> list)
            {
                if (result is null)
                {
                    throw new ArgumentNullException(nameof(result));
                }
                list = result.ToList();
            }
            InnerList = list;
            Paginator = paginator ?? throw new ArgumentNullException(nameof(paginator));
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
