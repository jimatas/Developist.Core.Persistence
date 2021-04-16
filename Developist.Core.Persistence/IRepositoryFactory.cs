// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface for a class that creates generic repositories for use inside a unit of work.
    /// </summary>
    public interface IRepositoryFactory
    {
        /// <summary>
        /// Creates a new generic repository to retrieve and persist entities of type <typeparamref name="TEntity"/> with.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to create a repository for.</typeparam>
        /// <param name="uow">The unit of work the repository will be enrolled in.</param>
        /// <returns>A new repository for the specified entity type.</returns>
        IRepository<TEntity> Create<TEntity>(IUnitOfWork uow) where TEntity : class, IEntity;
    }
}
