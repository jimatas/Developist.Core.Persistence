// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.EntityFramework
{
    public class Repository<TEntity, TDbContext> : IRepository<TEntity> 
        where TEntity : class, IEntity 
        where TDbContext : DbContext
    {
        private readonly IUnitOfWork<TDbContext> uow;

        public Repository(IUnitOfWork<TDbContext> uow)
        {
            this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public IUnitOfWork UnitOfWork => uow;

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter)
        {
            return Find(filter, includes: null);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes)
        {
            return CreateQuery(includes).Filter(filter).ToList();
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator)
        {
            return Find(filter, paginator, includes: null);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes)
        {
            return CreateQuery(includes).Filter(filter).Paginate(paginator).ToList();
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, includes: null, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default)
        {
            return await CreateQuery(includes).Filter(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, paginator, includes: null, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default)
        {
            return await CreateQuery(includes).Filter(filter).Paginate(paginator).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual void Add(TEntity entity)
        {
            uow.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
        }

        public virtual void Remove(TEntity entity, CancellationToken cancellationToken = default)
        {
            uow.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
        }

        protected IQueryable<TEntity> CreateQuery(IEntityIncludePaths<TEntity> includes)
        {
            var query = uow.DbContext.Set<TEntity>().AsQueryable();
            if (includes is not null && includes.Any())
            {
                query = includes.Distinct().Aggregate(query, (query, path) => query.Include(path));
            }

            return query;
        }
    }
}
