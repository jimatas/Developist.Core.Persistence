// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;

namespace Developist.Core.Persistence
{
    internal static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> sequence, IQueryableFilter<T> filter)
        {
            if (filter is null)
            {
                throw new ArgumentNullException(nameof(filter));
            }

            return filter.Filter(sequence);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> sequence, IQueryablePaginator<T> paginator)
        {
            if (paginator is null)
            {
                throw new ArgumentNullException(nameof(paginator));
            }

            return paginator.Paginate(sequence);
        }
    }
}
