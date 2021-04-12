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

        public virtual void Add(TEntity entity)
        {
            uow.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
        }

        public virtual void Remove(TEntity entity)
        {
            uow.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter)
        {
            return Find(filter, includePaths: null);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths)
        {
            return CreateQuery(includePaths).Filter(filter).ToList();
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator)
        {
            return Find(filter, paginator, includePaths: null);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths)
        {
            return CreateQuery(includePaths).Filter(filter).Paginate(paginator).ToList();
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, includePaths: null, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return await CreateQuery(includePaths).Filter(filter).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, paginator, includePaths: null, cancellationToken);
        }

        public virtual async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return await CreateQuery(includePaths).Filter(filter).Paginate(paginator).ToListAsync(cancellationToken).ConfigureAwait(false);
        }

        protected IQueryable<TEntity> CreateQuery(IEntityIncludePaths<TEntity> includePaths)
        {
            var query = uow.DbContext.Set<TEntity>().AsQueryable();
            if (includePaths is not null && includePaths.Any())
            {
                query = includePaths.Distinct().Aggregate(query, (query, path) => query.Include(path));
            }

            return query;
        }
    }
}
