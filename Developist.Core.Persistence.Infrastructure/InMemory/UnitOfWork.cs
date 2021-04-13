// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class UnitOfWork : DisposableBase, IUnitOfWork
    {
        private readonly ConcurrentDictionary<Type, RepositoryWrapper> repositories = new();
        private readonly IRepositoryFactory repositoryFactory;

        public UnitOfWork(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        public event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        public virtual void Complete()
        {
            Completed?.Invoke(this, new UnitOfWorkCompletedEventArgs(this));
        }

        public virtual Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            Completed?.Invoke(this, new UnitOfWorkCompletedEventArgs(this));
            return Task.CompletedTask;
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        {
            var wrapper = repositories.GetOrAdd(typeof(TEntity), _ => new(repositoryFactory.Create<TEntity>(this)));
            return wrapper.Repository<TEntity>();
        }

        protected override void ReleaseManagedResources()
        {
            repositories.Clear();
            base.ReleaseManagedResources();
        }

        protected override ValueTask ReleaseManagedResourcesAsync()
        {
            repositories.Clear();
            return base.ReleaseManagedResourcesAsync();
        }
    }
}
