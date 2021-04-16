// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface that must be implemented by a class in order to realize the repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities retrieved and persisted through this repository.</typeparam>
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        /// <summary>
        /// The unit of work this repository is enrolled in.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Adds a new entity to the data store.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(TEntity entity);

        /// <summary>
        /// Removes a previously persisted entity from the data store.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(TEntity entity);
    }
}
