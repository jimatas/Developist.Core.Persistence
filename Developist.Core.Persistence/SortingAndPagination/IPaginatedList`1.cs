// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Represents a read-only list view of a single page of items from a larger result set.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    public interface IPaginatedList<T> : IReadOnlyList<T>
    {
        /// <summary>
        /// The 1-based page number.
        /// </summary>
        int PageNumber { get; }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        int PageSize { get; }

        /// <summary>
        /// The number of pages needed to paginate over the entire result set.
        /// </summary>
        int PageCount { get; }

        /// <summary>
        /// The total number of items in the result set.
        /// </summary>
        int ItemCount { get; }
    }
}
