using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Utilities;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly IDictionary<Type, RepositoryWrapper> repositories = new Dictionary<Type, RepositoryWrapper>();
        private readonly IRepositoryFactory repositoryFactory;

        protected UnitOfWorkBase(IRepositoryFactory repositoryFactory)
        {
            this.repositoryFactory = ArgumentNullExceptionHelper.ThrowIfNull(() => repositoryFactory);
        }

        public event EventHandler<UnitOfWorkCompletedEventArgs>? Completed;
        public abstract Task CompleteAsync(CancellationToken cancellationToken = default);
        protected void OnCompleted(UnitOfWorkCompletedEventArgs e) => Completed?.Invoke(this, e);

        public abstract bool IsTransactional { get; }
        public abstract Task BeginTransactionAsync(CancellationToken cancellationToken = default);

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

        protected bool IsDisposed { get; private set; }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            GC.SuppressFinalize(this);
        }

        protected virtual ValueTask DisposeAsyncCore()
        {
            if (!IsDisposed)
            {
                repositories.Clear();
                IsDisposed = true;
            }
            return default;
        }

        private readonly struct RepositoryWrapper
        {
            private readonly object repository;
            public RepositoryWrapper(object repository) => this.repository = repository;
            public IRepository<TEntity> Repository<TEntity>()
                where TEntity : IEntity
            {
                return (IRepository<TEntity>)repository;
            }
        }
    }
}
