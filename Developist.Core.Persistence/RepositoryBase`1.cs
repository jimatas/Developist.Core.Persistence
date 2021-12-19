// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public abstract class RepositoryBase<TEntity> : IRepository<TEntity>
        where TEntity : class, IEntity
    {
        protected RepositoryBase(IUnitOfWork uow)
            => UnitOfWork = Ensure.Argument.NotNull(uow, nameof(uow));

        public virtual IUnitOfWork UnitOfWork { get; }

        #region Synchronous methods
        public abstract void Add(TEntity entity);
        public abstract void Remove(TEntity entity);

        public virtual int Count()
            => CreateQuery(includePaths: null).Count();

        public virtual int Count(IQueryableFilter<TEntity> filter)
            => CreateQuery(includePaths: null).Filter(filter).Count();

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter)
            => Find(filter, includePaths: null);

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths)
            => CreateQuery(includePaths).Filter(filter).ToList();

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator)
            => Find(filter, paginator, includePaths: null);

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths)
            => CreateQuery(includePaths).Filter(filter).Paginate(paginator).ToList();
        #endregion

        #region Asynchronous methods
        public abstract Task<int> CountAsync(CancellationToken cancellationToken = default);
        public abstract Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
            => FindAsync(filter, includePaths: null, cancellationToken);

        public abstract Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default);

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
            => FindAsync(filter, paginator, includePaths: null, cancellationToken);

        public abstract Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default);
        #endregion

        protected abstract IQueryable<TEntity> CreateQuery(IIncludePathCollection<TEntity> includePaths = null);
    }
}
