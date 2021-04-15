// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface for a class that paginates a sequence of elements.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public interface IQueryablePaginator<T>
    {
        /// <summary>
        /// Paginates a sequence by partitioning its elements into one or more subsequences.
        /// </summary>
        /// <param name="sequence">The sequence to paginate.</param>
        /// <returns>A subsequence of the elements.</returns>
        IOrderedQueryable<T> Paginate(IQueryable<T> sequence);
    }
}
