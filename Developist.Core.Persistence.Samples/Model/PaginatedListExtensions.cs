// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Samples
{
    public static class PaginatedListExtensions
    {
        public static bool HasNextPage<T>(this IPaginatedList<T> list) => list.PageNumber < list.PageCount;
        public static bool HasPreviousPage<T>(this IPaginatedList<T> list) => list.PageNumber > 1;
    }
}
