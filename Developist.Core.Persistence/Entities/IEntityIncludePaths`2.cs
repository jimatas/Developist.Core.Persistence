// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Allows for specifying the navigation properties of <typeparamref name="TEntity"/> to eager load when fetching instances of it through a repository.
    /// </summary>
    /// <remarks>
    /// Use the Include and ThenInclude extension methods to specify the include paths using lambda expressions.
    /// </remarks>
    /// <typeparam name="TEntity">The type of the entity to fetch related data for.</typeparam>
    /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
    public interface IEntityIncludePaths<TEntity, out TProperty> : IEntityIncludePaths<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Overridden to append a path segment to the last include path that was added.
        /// </summary>
        /// <param name="path">An include path segment.</param>
        new void Add(string path);
    }
}
