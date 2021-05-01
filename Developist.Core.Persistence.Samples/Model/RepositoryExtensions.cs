// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Samples
{
    public static class RepositoryExtensions
    {
        public static IPaginatedList<TEntity> All<TEntity>(this IRepository<TEntity> repository, SortingPaginator<TEntity> paginator) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator), paginator);
        }

        public static IPaginatedList<TEntity> All<TEntity>(this IRepository<TEntity> repository, SortingPaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(repository.Find(PassthroughFilter<TEntity>.Default, paginator, includePaths), paginator);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IRepository<TEntity> repository, SortingPaginator<TEntity> paginator, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, cancellationToken), paginator);
        }

        public static async Task<IPaginatedList<TEntity>> AllAsync<TEntity>(this IRepository<TEntity> repository, SortingPaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            return new PaginatedList<TEntity>(await repository.FindAsync(PassthroughFilter<TEntity>.Default, paginator, includePaths, cancellationToken), paginator);
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
