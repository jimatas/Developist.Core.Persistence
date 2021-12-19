// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class Repository<TEntity> : RepositoryBase<TEntity>
        where TEntity : class, IEntity
    {
        public Repository(IUnitOfWork uow) : base(new UnitOfWorkDataStore(uow))
            => UnitOfWork.Completed += UnitOfWorkCompleted;

        public ISet<TEntity> DataStore { get; private set; } = new HashSet<TEntity>();

        public override void Add(TEntity entity)
        {
            Ensure.Argument.NotNull(entity, nameof(entity));

            ((UnitOfWorkDataStore)UnitOfWork).DataStore.Add(entity);
        }

        public override void Remove(TEntity entity)
        {
            Ensure.Argument.NotNull(entity, nameof(entity));

            ((UnitOfWorkDataStore)UnitOfWork).DataStore.Remove(entity);
        }

        public override Task<int> CountAsync(CancellationToken cancellationToken = default) 
            => Task.FromResult(Count());

        public override Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default) 
            => Task.FromResult(Count(filter));

        public override Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default)
            => Task.FromResult(Find(filter, includePaths));

        public override Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default)
            => Task.FromResult(Find(filter, paginator, includePaths));

        protected override IQueryable<TEntity> CreateQuery(IIncludePathCollection<TEntity> includePaths = null) 
            => DataStore.AsQueryable();

        private void UnitOfWorkCompleted(object sender, UnitOfWorkCompletedEventArgs e)
            => DataStore = new HashSet<TEntity>(((UnitOfWorkDataStore)UnitOfWork).DataStore);

        private class UnitOfWorkDataStore : IUnitOfWork
        {
            private readonly IUnitOfWork uow;

            public event EventHandler<UnitOfWorkCompletedEventArgs> Completed
            {
                add => uow.Completed += value;
                remove => uow.Completed -= value;
            }

            public UnitOfWorkDataStore(IUnitOfWork uow, IEnumerable<TEntity> dataStore = null)
            {
                this.uow = Ensure.Argument.NotNull(uow, nameof(uow));
                DataStore = new HashSet<TEntity>(dataStore ?? Array.Empty<TEntity>());
            }

            public ISet<TEntity> DataStore { get; }
            public bool IsTransactional => uow.IsTransactional;

            public void BeginTransaction() => uow.BeginTransaction();
            public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => uow.BeginTransactionAsync(cancellationToken);
            public void Complete() => uow.Complete();
            public Task CompleteAsync(CancellationToken cancellationToken = default) => uow.CompleteAsync(cancellationToken);
            public IRepository<T> Repository<T>() where T : class, IEntity => uow.Repository<T>();

            public void Dispose() => uow.Dispose();
        }
    }
}
