// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface to implement by a class in order to realize the unit of work pattern.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The event that is fired when the unit of work completes.
        /// </summary>
        event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        /// <summary>
        /// Completes the unit of work by persisting all changes made inside of it to the data store.
        /// </summary>
        void Complete();

        /// <summary>
        /// Async version of <see cref="Complete"/>
        /// <para>
        /// Completes the unit of work by persisting all changes made inside of it to the data store.
        /// </para>
        /// </summary>
        Task CompleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a generic repository to retrieve and persist entities of type <typeparamref name="TEntity"/> with.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to return a repository for.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
    }
}
