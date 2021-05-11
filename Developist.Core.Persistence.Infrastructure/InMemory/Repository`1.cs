﻿// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

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
            Ensure.Argument.NotNull(entity, nameof(entity));

            uow.DataStore.Add(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            Ensure.Argument.NotNull(entity, nameof(entity));

            uow.DataStore.Remove(entity);
        }

        public virtual int Count()
        {
            return DataStore.Count;
        }

        public virtual int Count(IQueryableFilter<TEntity> filter)
        {
            return DataStore.AsQueryable().Filter(filter).Count();
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter)
        {
            return DataStore.AsQueryable().Filter(filter);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths)
        {
            return Find(filter);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator)
        {
            return DataStore.AsQueryable().Filter(filter).Paginate(paginator);
        }

        public virtual IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths)
        {
            return Find(filter, paginator);
        }

        public virtual Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Count());
        }

        public virtual Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Count(filter));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, includePaths));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, paginator));
        }

        public virtual Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Find(filter, paginator, includePaths));
        }

        private void UnitOfWorkCompleted(object sender, UnitOfWorkCompletedEventArgs e)
        {
            DataStore = new HashSet<TEntity>(uow.DataStore);
        }

        protected class UnitOfWorkDataStore : IUnitOfWork
        {
            private readonly IUnitOfWork uow;

            public UnitOfWorkDataStore(IUnitOfWork uow, IEnumerable<TEntity> dataStore = null)
            {
                this.uow = Ensure.Argument.NotNull(uow, nameof(uow));
                DataStore = new HashSet<TEntity>(dataStore ?? Array.Empty<TEntity>());
            }

            public ISet<TEntity> DataStore { get; }

            public event EventHandler<UnitOfWorkCompletedEventArgs> Completed
            {
                add => uow.Completed += value;
                remove => uow.Completed -= value;
            }

            public bool IsTransactional => uow.IsTransactional;

            public void BeginTransaction() => uow.BeginTransaction();
            public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => uow.BeginTransactionAsync(cancellationToken);
            public void Complete() => uow.Complete();
            public Task CompleteAsync(CancellationToken cancellationToken = default) => uow.CompleteAsync(cancellationToken);
            public IRepository<T> Repository<T>() where T : class, IEntity => uow.Repository<T>();

            public void Dispose() => uow.Dispose();
            public ValueTask DisposeAsync() => uow.DisposeAsync();
        }
    }
}
