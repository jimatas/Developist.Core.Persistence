// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public abstract class UnitOfWorkBase : DisposableBase, IUnitOfWork
    {
        public event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        private readonly IDictionary<Type, RepositoryWrapper> repositories = new Dictionary<Type, RepositoryWrapper>();
        private readonly IRepositoryFactory repositoryFactory;

        protected UnitOfWorkBase(IRepositoryFactory repositoryFactory)
            => this.repositoryFactory = Ensure.Argument.NotNull(repositoryFactory, nameof(repositoryFactory));

        public abstract bool IsTransactional { get; }

        public abstract void BeginTransaction();
        public virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            BeginTransaction();
            return Task.CompletedTask;
        }

        public abstract void Complete();
        public abstract Task CompleteAsync(CancellationToken cancellationToken = default);

        public IRepository<TEntity> Repository<TEntity>()
            where TEntity : class, IEntity
        {
            if (!repositories.TryGetValue(typeof(TEntity), out RepositoryWrapper wrapper))
            {
                wrapper = new RepositoryWrapper(repositoryFactory.Create<TEntity>(this));
                repositories.Add(typeof(TEntity), wrapper);
            }
            return wrapper.Repository<TEntity>();
        }

        protected void OnCompleted(UnitOfWorkCompletedEventArgs e) => Completed?.Invoke(this, e);

        protected override void ReleaseManagedResources()
        {
            repositories.Clear();
            base.ReleaseManagedResources();
        }

        private class RepositoryWrapper
        {
            private readonly object repository;
            public RepositoryWrapper(object repository) => this.repository = Ensure.Argument.NotNull(repository, nameof(repository));
            public IRepository<TEntity> Repository<TEntity>()
                where TEntity : class, IEntity => (IRepository<TEntity>)repository;
        }
    }
}
