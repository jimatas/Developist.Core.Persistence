// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// A sorting directive to apply to a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public interface ISortDirective<T>
    {
        /// <summary>
        /// Applies the sorting directive encapsulated by this instance to the specified sequence.
        /// </summary>
        /// <param name="sequence">The sequence to sort.</param>
        /// <returns>The resulting sorted sequence.</returns>
        IOrderedQueryable<T> ApplyTo(IQueryable<T> sequence);
    }
}
