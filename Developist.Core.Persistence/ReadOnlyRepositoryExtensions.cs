using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.Pagination;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public static class ReadOnlyRepositoryExtensions
    {
        public static Task<int> CountAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return repository.CountAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken);
        }

        public static async Task<TEntity?> GetAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static async Task<TEntity?> GetAsync<TEntity, TIdentifier>(this IReadOnlyRepository<TEntity> repository, TIdentifier id, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : class, IEntity<TIdentifier>
            where TIdentifier : IEquatable<TIdentifier>
        {
            return (await repository.FindAsync(entity => entity.Id.Equals(id), configureIncludePaths, cancellationToken).ConfigureAwait(false)).SingleOrDefault();
        }

        public static Task<IReadOnlyList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), cancellationToken);
        }

        public static Task<IReadOnlyList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var includePaths = new IncludePathsBuilder<TEntity>();
            configureIncludePaths(includePaths);

            return repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), includePaths, cancellationToken);
        }

        public static async Task<IReadOnlyPaginatedList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            var list = await repository.FindAsync(predicate, paginator, cancellationToken).ConfigureAwait(false);
            return list.ToPaginatedList(paginator);
        }

        public static Task<IReadOnlyList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            return repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, cancellationToken);
        }

        public static async Task<IReadOnlyPaginatedList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, Action<SortingPaginator<TEntity>> configurePaginator, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            var list = await repository.FindAsync(predicate, paginator, configureIncludePaths, cancellationToken).ConfigureAwait(false);
            return list.ToPaginatedList(paginator);
        }

        public static Task<IReadOnlyList<TEntity>> FindAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Expression<Func<TEntity, bool>> predicate, IQueryablePaginator<TEntity> paginator, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var includePaths = new IncludePathsBuilder<TEntity>();
            configureIncludePaths(includePaths);

            return repository.FindAsync(new PredicateQueryableFilter<TEntity>(predicate), paginator, includePaths, cancellationToken);
        }

        public static Task<IReadOnlyPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.AllAsync(paginator, cancellationToken);
        }

        public static async Task<IReadOnlyPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var list = await repository.FindAsync(PassthroughQueryableFilter<TEntity>.Default, paginator, cancellationToken).ConfigureAwait(false);
            return list.ToPaginatedList(paginator);
        }

        public static Task<IReadOnlyPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, Action<SortingPaginator<TEntity>> configurePaginator, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var paginator = new SortingPaginator<TEntity>();
            configurePaginator(paginator);

            return repository.AllAsync(paginator, configureIncludePaths, cancellationToken);
        }

        public static async Task<IReadOnlyPaginatedList<TEntity>> AllAsync<TEntity>(this IReadOnlyRepository<TEntity> repository, SortingPaginator<TEntity> paginator, Action<IIncludePathsBuilder<TEntity>> configureIncludePaths, CancellationToken cancellationToken = default)
            where TEntity : IEntity
        {
            var includePaths = new IncludePathsBuilder<TEntity>();
            configureIncludePaths(includePaths);

            var list = await repository.FindAsync(PassthroughQueryableFilter<TEntity>.Default, paginator, includePaths, cancellationToken).ConfigureAwait(false);
            return list.ToPaginatedList(paginator);
        }

        private class PredicateQueryableFilter<TEntity> : IQueryableFilter<TEntity>
            where TEntity : IEntity
        {
            private readonly Expression<Func<TEntity, bool>> predicate;
            public PredicateQueryableFilter(Expression<Func<TEntity, bool>> predicate) => this.predicate = predicate;
            public IQueryable<TEntity> Filter(IQueryable<TEntity> query) => query.Where(predicate);
        }

        private class PassthroughQueryableFilter<TEntity> : IQueryableFilter<TEntity>
            where TEntity : IEntity
        {
            public static readonly IQueryableFilter<TEntity> Default = new PassthroughQueryableFilter<TEntity>();
            public IQueryable<TEntity> Filter(IQueryable<TEntity> query) => query;
        }
    }
}
