// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence.Pagination
{
    public interface IPaginatedList<T> : IReadOnlyList<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        int PageCount { get; }
        int ItemCount { get; }
    }
}
