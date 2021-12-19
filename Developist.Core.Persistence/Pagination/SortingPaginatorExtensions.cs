// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    public static class SortingPaginatorExtensions
    {
        public static SortingPaginator<T> StartingAt<T>(this SortingPaginator<T> paginator, int pageNumber)
        {
            paginator.PageNumber = pageNumber;
            return paginator;
        }

        public static SortingPaginator<T> WithPageSizeOf<T>(this SortingPaginator<T> paginator, int pageSize)
        {
            paginator.PageSize = pageSize;
            return paginator;
        }

        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string propertyName, SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortDirectives.Add(new SortProperty<T>(propertyName, direction));
            return paginator;
        }

        public static SortingPaginator<T> SortedBy<T, TProperty>(this SortingPaginator<T> paginator, Expression<Func<T, TProperty>> property, SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortDirectives.Add(new SortProperty<T, TProperty>(property, direction));
            return paginator;
        }

        public static SortingPaginator<T> SortedByString<T>(this SortingPaginator<T> paginator, string value)
        {
            foreach (var property in SortPropertyHelper<T>.ParseFromString(value))
            {
                paginator.SortDirectives.Add(property);
            }
            return paginator;
        }
    }
}
