// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public static partial class RepositoryExtensions
    {
        /// <summary>
        /// Returns an <see cref="IPaginatedList{T}"/> representing a subset of all the entities of generic type <typeparamref name="TEntity"/> in the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="paginator">The pagination instructions.</param>
        /// <returns>The specified range of <typeparamref name="TEntity"/> entities that were retrieved from the data store.</returns>
        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator), paginator);
        }

        /// <summary>
        /// Returns an <see cref="IPaginatedList{T}"/> representing a subset of all the entities of generic type <typeparamref name="TEntity"/> in the data store.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="paginator">The pagination instructions.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>The specified range of <typeparamref name="TEntity"/> entities that were retrieved from the data store.</returns>
        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator, includePaths), paginator);
        }

        /// <summary>
        /// Async version of <see cref="All{TEntity}(IRepository{TEntity}, SortingPaginator{TEntity})"/>
        /// <para>
        /// Returns an <see cref="IPaginatedList{T}"/> representing a subset of all the entities of generic type <typeparamref name="TEntity"/> in the data store.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="paginator">The pagination instructions.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the specified range of <typeparamref name="TEntity"/> entities that were retrieved from the data store.</returns>
        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, cancellationToken).ConfigureAwait(false), paginator);
        }

        /// <summary>
        /// Async version of <see cref="All{TEntity}(IRepository{TEntity}, SortingPaginator{TEntity}, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Returns an <see cref="IPaginatedList{T}"/> representing a subset of all the entities of generic type <typeparamref name="TEntity"/> in the data store.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="paginator">The pagination instructions.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the specified range of <typeparamref name="TEntity"/> entities that were retrieved from the data store.</returns>
        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, includePaths, cancellationToken).ConfigureAwait(false), paginator);
        }

        /// <summary>
        /// Does not filter any elements.
        /// </summary>
        private class PassthroughFilter<T> : IQueryableFilter<T>
        {
            public static readonly IQueryableFilter<T> Default = new PassthroughFilter<T>();
            public IQueryable<T> Filter(IQueryable<T> sequence) => sequence;
        }
    }
}
