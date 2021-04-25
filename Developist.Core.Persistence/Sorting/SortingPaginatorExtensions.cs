// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public static class SortingPaginatorExtensions
    {
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string propertyName) => paginator.SortedBy(propertyName, SortDirection.Ascending);
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string propertyName, SortDirection direction)
        {
            paginator.SortProperties.Add(new(propertyName, direction));
            return paginator;
        }

        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, Expression<Func<T, object>> propertySelector) => paginator.SortedBy(propertySelector, SortDirection.Ascending);
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, Expression<Func<T, object>> propertySelector, SortDirection direction)
        {
            paginator.SortProperties.Add(new(propertySelector, direction));
            return paginator;
        }
    }
}
