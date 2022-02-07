// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using Microsoft.EntityFrameworkCore;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public class Repository<TEntity, TDbContext> : RepositoryBase<TEntity>
        where TEntity : class, IEntity
        where TDbContext : DbContext
    {
        public Repository(IUnitOfWork<TDbContext> uow) : base(uow) { }

        public override IUnitOfWork<TDbContext> UnitOfWork => (IUnitOfWork<TDbContext>)base.UnitOfWork;

        public override void Add(TEntity entity)
        {
            Ensure.Argument.NotNull(entity, nameof(entity));

            UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
        }

        public override void Remove(TEntity entity)
        {
            Ensure.Argument.NotNull(entity, nameof(entity));

            UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
        }

        public override async Task<int> CountAsync(CancellationToken cancellationToken = default) 
            => await CreateQuery(includePaths: null).CountAsync(cancellationToken).WithoutCapturingContext();

        public override async Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default) 
            => await CreateQuery(includePaths: null).Filter(filter).CountAsync(cancellationToken).WithoutCapturingContext();

        public override async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default) 
            => await CreateQuery(includePaths).Filter(filter).ToListAsync(cancellationToken).WithoutCapturingContext();

        public override async Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default) 
            => await CreateQuery(includePaths).Filter(filter).Paginate(paginator).ToListAsync(cancellationToken).WithoutCapturingContext();

        protected override IQueryable<TEntity> CreateQuery(IIncludePathCollection<TEntity> includePaths = null)
        {
            var query = UnitOfWork.DbContext.Set<TEntity>().AsQueryable();
            if (includePaths is not null && includePaths.Any())
            {
                query = includePaths.Distinct().Aggregate(query, (query, path) => query.Include(path));
            }
            return query;
        }
    }
}
