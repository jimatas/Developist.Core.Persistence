// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Utilities;

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
        #region Synchronous extension methods
        public static int Count<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : IEntity
        {
            return repository.Count(new PredicateQueryableFilter<TEntity>(predicate));
        }

        public static TEntity Get<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id)).SingleOrDefault();
        }

        public static TEntity Get<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return repository.Find(entity => entity.Id.Equals(id), configureIncludePaths).SingleOrDefault();
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate)
            where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate));
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), includePaths);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.Find(predicate, paginator);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, SortingPaginator<TEntity> paginator)
            where TEntity : IEntity
        {
            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.Find(predicate, paginator, configureIncludePaths);
        }

        public static IEnumerable<TEntity> Find<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return repository.Find(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths);
        }

        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator)
            where TEntity : class, IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.All(paginator);
        }

        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator)
            where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator), paginator);
        }

        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : class, IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.All(paginator, configureIncludePaths);
        }

        public static IPaginatedList<TEntity> All<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths)
            where TEntity : class, IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator, includePaths), paginator);
        }
        #endregion

        #region Asynchronous extension methods
        public static async Task<int> CountAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return await repository.CountAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<TEntity> GetAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static async Task<TEntity> GetAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), configureIncludePaths, cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), includePaths, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return await repository.FindAsync(predicate, paginator, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return await repository.FindAsync(predicate, paginator, configureIncludePaths, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IEnumerable<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return await repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return await repository.AllAsync(paginator, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, cancellationToken).ConfigureAwait(false), paginator);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return await repository.AllAsync(paginator, configureIncludePaths, cancellationToken).ConfigureAwait(false);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, Func<IIncludePathCollection<TEntity>, IIncludePathCollection<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity
        {
            var includePaths = configureIncludePaths(new IncludePathCollection<TEntity>());

            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, includePaths, cancellationToken).ConfigureAwait(false), paginator);
        }
        #endregion

        private class PredicateQueryableFilter<T> : IQueryableFilter<T>
        {
            private readonly Expression<Func<T, bool>> predicate;
            public PredicateQueryableFilter(Expression<Func<T, bool>> predicate) => this.predicate = Ensure.Argument.NotNull(predicate, nameof(predicate));
            public IQueryable<T> Filter(IQueryable<T> sequence) => sequence.Where(predicate);
        }

        private class PassthroughFilter<T> : IQueryableFilter<T>
        {
            public static readonly IQueryableFilter<T> Default = new PassthroughFilter<T>();
            public IQueryable<T> Filter(IQueryable<T> sequence) => sequence;
        }
    }
}
