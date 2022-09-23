using Developist.Core.Persistence;
using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Utilities;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public class Repository<TEntity, TDbContext> : IRepository<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        public Repository(IUnitOfWork<TDbContext> unitOfWork)
        {
            UnitOfWork = ArgumentNullExceptionHelper.ThrowIfNull(() => unitOfWork);
        }

        IUnitOfWork IRepository<TEntity>.UnitOfWork => UnitOfWork;
        public IUnitOfWork<TDbContext> UnitOfWork { get; }

        public void Add(TEntity entity)
        {
            UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
        }

        public void Remove(TEntity entity)
        {
            UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return CreateQuery().CountAsync(cancellationToken);
        }

        public Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return CreateQuery().Filter(filter).CountAsync(cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return CreateQuery().Filter(filter).ToReadOnlyListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return CreateQuery(includePaths).Filter(filter).ToReadOnlyListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return CreateQuery().Filter(filter).Paginate(paginator).ToReadOnlyListAsync(cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return CreateQuery(includePaths).Filter(filter).Paginate(paginator).ToReadOnlyListAsync(cancellationToken);
        }

        protected IQueryable<TEntity> CreateQuery(IIncludePathsBuilder<TEntity>? includePaths = null)
        {
            var query = UnitOfWork.DbContext.Set<TEntity>().AsQueryable();
            query = includePaths?.ToArray().Distinct().Aggregate(query, (query, path) => query.Include(path)) ?? query;
            return query;
        }
    }
}
