// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence
{
    internal static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> sequence, IQueryableFilter<T> filter)
        {
            return filter.Filter(sequence);
        }

        public static IQueryable<T> Paginate<T>(this IQueryable<T> sequence, IQueryablePaginator<T> paginator)
        {
            return paginator.Paginate(sequence);
        }
    }
}
