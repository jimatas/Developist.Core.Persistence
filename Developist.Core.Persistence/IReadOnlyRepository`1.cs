// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines (the retrieval part of) the interface to implement by a class in order to realize the repository pattern.
    /// </summary>
    /// <typeparam name="TEntity">The type of entities retrieved through this repository.</typeparam>
    public interface IReadOnlyRepository<TEntity> where TEntity : IEntity
    {
        /// <summary>
        /// Fetches all entities from the data store that pass the specified filter criteria.
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <returns>The result set of entities.</returns>
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter);

        /// <summary>
        /// Fetches all entities from the data store that pass the specified filter criteria.
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>The result set of entities.</returns>
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths);

        /// <summary>
        /// Fetches a subset of the entities from the data store that pass the specified filter criteria.
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <returns>A subset of the entities to return.</returns>
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator);

        /// <summary>
        /// Fetches a subset of the entities from the data store that pass the specified filter criteria.
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>A subset of the entities to return.</returns>
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths);

        /// <summary>
        /// Async version of <see cref="Find(IQueryableFilter{TEntity})"/>
        /// <para>
        /// Fetches all entities from the data store that pass the specified filter criteria.
        /// </para>
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the result set of entities.</returns>
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async version of <see cref="Find(IQueryableFilter{TEntity}, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Fetches all entities from the data store that pass the specified filter criteria.
        /// </para>
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the result set of entities.</returns>
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async version of <see cref="Find(IQueryableFilter{TEntity}, IQueryablePaginator{TEntity})"/>
        /// <para>
        /// Fetches a subset of the entities from the data store that pass the specified filter criteria.
        /// </para>
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the subset of entities to return.</returns>
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default);

        /// <summary>
        /// Async version of <see cref="Find(IQueryableFilter{TEntity}, IQueryablePaginator{TEntity}, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Fetches a subset of the entities from the data store that pass the specified filter criteria.
        /// </para>
        /// </summary>
        /// <param name="filter">The criteria by which to filter the entities to return.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</par
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the subset of entities to return.</returns>
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default);
    }
}
