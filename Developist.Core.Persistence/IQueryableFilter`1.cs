// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface for a class that filters the elements in a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public interface IQueryableFilter<T>
    {
        /// <summary>
        /// Returns a new sequence containing only those elements from the original sequence that pass the filter.
        /// </summary>
        /// <param name="sequence">The sequence to filter.</param>
        /// <returns>A filtered sequence.</returns>
        IQueryable<T> Filter(IQueryable<T> sequence);
    }
}
