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
        public static TEntity Find<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id)).SingleOrDefault();
        }

        public static TEntity Find<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, IEntityIncludePaths<TEntity> includePaths)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id), includePaths).SingleOrDefault();
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate));
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IEntityIncludePaths<TEntity> includePaths) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), includePaths);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths) where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths);
        }

        public static async Task<TEntity> FindAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static async Task<TEntity> FindAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), includePaths, cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), includePaths, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default) where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, cancellationToken).ConfigureAwait(false);
        }

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
