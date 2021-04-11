// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class, IEntity
    {
        private readonly UnitOfWorkDataStore uow;

        public Repository(IUnitOfWork uow)
        {
            this.uow = new UnitOfWorkDataStore(uow, DataStore);
            this.uow.Completed += UnitOfWorkCompleted;
        }

        public IUnitOfWork UnitOfWork => uow;
        public ISet<TEntity> DataStore { get; private set; } = new HashSet<TEntity>();

        public virtual void Add(TEntity entity)
        {
            uow.DataStore.Add(entity);
        }

        public virtual void Remove(TEntity entity, CancellationToken cancellationToken = default)
        {
            uow.DataStore.Remove(entity);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter)
        {
            return filter.Filter(DataStore.AsQueryable());
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes)
        {
            return Find(filter);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator)
        {
            return filter.Filter(DataStore.AsQueryable()).Paginate(paginator);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes)
        {
            return Find(filter, paginator);
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, includes));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, paginator));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, paginator, includes));
        }

        private void UnitOfWorkCompleted(object sender, UnitOfWorkEventArgs e)
        {
            if (e.UnitOfWork is UnitOfWorkDataStore uow)
            {
                DataStore = new HashSet<TEntity>(uow.DataStore);
                // uow.DataStore.Clear();
            }
        }

        protected class UnitOfWorkDataStore : IUnitOfWork
        {
            private readonly IUnitOfWork uow;
            public UnitOfWorkDataStore(IUnitOfWork uow, IEnumerable<TEntity> dataStore = null)
            {
                this.uow = uow ?? throw new ArgumentNullException(nameof(uow));
                DataStore = new HashSet<TEntity>(dataStore ?? Array.Empty<TEntity>());
            }

            public ISet<TEntity> DataStore { get; }

            public event EventHandler<UnitOfWorkEventArgs> Completed
            {
                add => uow.Completed += value;
                remove => uow.Completed -= value;
            }
            public void Complete() => uow.Complete();
            public Task CompleteAsync(CancellationToken cancellationToken = default) => uow.CompleteAsync(cancellationToken);
            public IRepository<T> Repository<T>() where T : class, IEntity => uow.Repository<T>();
        }
    }
}
