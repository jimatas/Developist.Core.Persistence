// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface that must be implemented by a class in order to realize the unit of work pattern.
    /// </summary>
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// The event that is fired when the unit of work completes.
        /// </summary>
        event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        /// <summary>
        /// Indicates whether an explicit transaction has been started on this unit of work through the <see cref="BeginTransaction"/> or <see cref="BeginTransactionAsync"/> method.
        /// </summary>
        bool IsTransactional { get; }

        /// <summary>
        /// If supported by the data store, will start an explicit transaction in which all the write operations will be wrapped upon the next call to <see cref="Complete"/> or <see cref="CompleteAsync"/>.
        /// </summary>
        /// <remarks>
        /// This method may throw an exception when it is called while an active transaction is already underway.
        /// Consult the <see cref="IsTransactional"/> property to see if that is the case.
        /// </remarks>
        void BeginTransaction();

        /// <summary>
        /// Async version of <see cref="BeginTransaction"/>
        /// <para>
        /// If supported by the data store, will start an explicit transaction in which all the write operations will be wrapped upon the next call to <see cref="Complete"/> or <see cref="CompleteAsync"/>.
        /// </para>
        /// </summary>
        /// <remarks>
        /// This method may throw an exception when it is called while an active transaction is already underway.
        /// Consult the <see cref="IsTransactional"/> property to see if that is the case.
        /// </remarks>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

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
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation.</returns>
        Task CompleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a generic repository to retrieve and persist entities of type <typeparamref name="TEntity"/> with.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to return a repository for.</typeparam>
        /// <returns>A repository for the specified entity type.</returns>
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
    }
}
