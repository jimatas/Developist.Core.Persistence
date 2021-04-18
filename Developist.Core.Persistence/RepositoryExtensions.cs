// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Counts the number of entities of generic type <typeparamref name="TEntity"/> in the data store that satisfy the specified condition.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to count.</typeparam>
        /// <param name="repository">The repository with which to count the entities.</param>
        /// <param name="predicate">The criteria by which to filter the entities to include in the count, specified as a predicate expression.</param>
        /// <returns>The number of entities counted.</returns>
        public static int Count<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity
        {
            return repository.Count(new PredicateQueryableFilter<TEntity>(predicate));
        }

        /// <summary>
        /// Rerieves a single entity from the data store using its unique Id property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
        /// <param name="repository">The repository to retrieve the entity with.</param>
        /// <param name="id">The value of the entity's Id property.</param>
        /// <returns>The entity for the specified Id value, or <see langword="null"/> if no such entity exists.</returns>
        public static TEntity Find<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id)).SingleOrDefault();
        }

        /// <summary>
        /// Rerieves a single entity from the data store using its unique Id property.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
        /// <param name="repository">The repository to retrieve the entity with.</param>
        /// <param name="id">The value of the entity's Id property.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>The entity for the specified Id value, or <see langword="null"/> if no such entity exists.</returns>
        public static TEntity Find<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, IEntityIncludePaths<TEntity> includePaths)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id), includePaths).SingleOrDefault();
        }

        /// <summary>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <returns>The entities that were retrieved.</returns>
        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate));
        }

        /// <summary>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>The entities that were retrieved.</returns>
        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IEntityIncludePaths<TEntity> includePaths) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), includePaths);
        }

        /// <summary>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <returns>The entities that were retrieved.</returns>
        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator);
        }

        /// <summary>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <returns>The entities that were retrieved.</returns>
        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths);
        }

        /// <summary>
        /// Async version of <see cref="Count{TEntity}(IReadOnlyRepository{TEntity}, Expression{Func{TEntity, bool}})"/>
        /// <para>
        /// Counts the number of entities of generic type <typeparamref name="TEntity"/> in the data store that satisfy the specified condition.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to count.</typeparam>
        /// <param name="repository">The repository with which to count the entities.</param>
        /// <param name="predicate">The criteria by which to filter the entities to include in the count, specified as a predicate expression.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the number of entities counted.</returns>
        public static async Task<int> CountAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.CountAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity, TIdentifier}(IReadOnlyRepository{TEntity}, TIdentifier)"/>
        /// <para>
        /// Rerieves a single entity from the data store using its unique Id property.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
        /// <param name="repository">The repository to retrieve the entity with.</param>
        /// <param name="id">The value of the entity's Id property.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entity for the specified Id value, or <see langword="null"/> if no such entity exists.</returns>
        public static async Task<TEntity> FindAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity, TIdentifier}(IReadOnlyRepository{TEntity}, TIdentifier, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Rerieves a single entity from the data store using its unique Id property.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to retrieve.</typeparam>
        /// <typeparam name="TIdentifier">The type of the entity's identifier.</typeparam>
        /// <param name="repository">The repository to retrieve the entity with.</param>
        /// <param name="id">The value of the entity's Id property.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entity for the specified Id value, or <see langword="null"/> if no such entity exists.</returns>
        public static async Task<TEntity> FindAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), includePaths, cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity}(IReadOnlyRepository{TEntity}, Expression{Func{TEntity, bool}})"/>
        /// <para>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entities that were retrieved.</returns>
        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity}(IReadOnlyRepository{TEntity}, Expression{Func{TEntity, bool}}, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entities that were retrieved.</returns>
        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), includePaths, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity}(IReadOnlyRepository{TEntity}, Expression{Func{TEntity, bool}}, IQueryablePaginator{TEntity})"/>
        /// <para>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entities that were retrieved.</returns>
        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, cancellationToken).ConfigureAwait(false);
        }

        /// <summary>
        /// Async version of <see cref="Find{TEntity}(IReadOnlyRepository{TEntity}, Expression{Func{TEntity, bool}}, IQueryablePaginator{TEntity}, IEntityIncludePaths{TEntity})"/>
        /// <para>
        /// Supports the retrieval of entities from the data store using a predicate expression.
        /// </para>
        /// </summary>
        /// <typeparam name="TEntity">The type of the entities to retrieve.</typeparam>
        /// <param name="repository">The repository to retrieve the entities with.</param>
        /// <param name="predicate">The criteria by which to filter the entities to return, specified as a predicate expression.</param>
        /// <param name="paginator">The pagination instructions to apply to the result set.</param>
        /// <param name="includePaths">The navigation properties of <typeparamref name="TEntity"/> to include in the result.</param>
        /// <param name="cancellationToken">The cancellation token to observe.</param>
        /// <returns>An awaitable task representing the asynchronous operation. The task result will contain the entities that were retrieved.</returns>
        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths, cancellationToken).ConfigureAwait(false);
        }

        private class PredicateQueryableFilter<T> : IQueryableFilter<T>
        {
            private readonly Expression<Func<T, bool>> predicate;
            public PredicateQueryableFilter(Expression<Func<T, bool>> predicate) => this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
            public IQueryable<T> Filter(IQueryable<T> sequence) => sequence.Where(predicate);
        }
    }
}
