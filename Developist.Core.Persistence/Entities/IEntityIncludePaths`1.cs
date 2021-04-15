// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Allows for specifying the navigation properties of <typeparamref name="TEntity"/> to eager load when fetching instances of it through a repository.
    /// </summary>
    /// <remarks>
    /// Use the Include and ThenInclude extension methods to specify the include paths using lambda expressions.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity to fetch related data for.</typeparam>
    public interface IEntityIncludePaths<TEntity> : IEnumerable<string> where TEntity : IEntity
    {
        /// <summary>
        /// Adds an include path.
        /// </summary>
        /// <param name="path">An include path.</param>
        void Add(string path);

        /// <summary>
        /// Removes a previously added include path.
        /// </summary>
        /// <remarks>
        /// If multiple occurrences of <paramref name="path"/> exist, removes the last occurrence.
        /// </remarks>
        /// <param name="path">An include path.</param>
        void Remove(string path);
    }
}
