using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class Repository<TEntity> : IRepository<TEntity>
        where TEntity : IEntity
    {
        private static readonly IIncludePathsBuilder<TEntity> DefaultIncludePaths = new IncludePathsBuilder<TEntity>();

        public Repository(IUnitOfWork unitOfWork)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => unitOfWork);
            UnitOfWork = new UnitOfWorkWithDataStore(unitOfWork);
            UnitOfWork.Completed += UnitOfWorkCompleted;
        }

        public ISet<TEntity> DataStore { get; private set; } = new HashSet<TEntity>();
        public IUnitOfWork UnitOfWork { get; }

        public void Add(TEntity entity)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => entity);
            ((UnitOfWorkWithDataStore)UnitOfWork).DataStore.Add(entity);
        }

        public void Remove(TEntity entity)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => entity);
            ((UnitOfWorkWithDataStore)UnitOfWork).DataStore.Remove(entity);
        }

        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DataStore.Count);
        }

        public Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            var query = DataStore.AsQueryable().Filter(filter);
            return Task.FromResult(query.Count());
        }

        public virtual Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, DefaultIncludePaths, cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DataStore.AsQueryable().Filter(filter).ToReadOnlyList());
        }

        public virtual Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default)
        {
            return FindAsync(filter, paginator, DefaultIncludePaths, cancellationToken);
        }

        public Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DataStore.AsQueryable().Filter(filter).Paginate(paginator).ToReadOnlyList());
        }

        private void UnitOfWorkCompleted(object sender, UnitOfWorkCompletedEventArgs e)
        {
            DataStore = new HashSet<TEntity>(((UnitOfWorkWithDataStore)UnitOfWork).DataStore);
        }

        private class UnitOfWorkWithDataStore : IUnitOfWork
        {
            private readonly IUnitOfWork unitOfWork;

            public event EventHandler<UnitOfWorkCompletedEventArgs>? Completed
            {
                add => unitOfWork.Completed += value;
                remove => unitOfWork.Completed -= value;
            }

            public UnitOfWorkWithDataStore(IUnitOfWork unitOfWork) => this.unitOfWork = unitOfWork;

            public ISet<TEntity> DataStore { get; } = new HashSet<TEntity>();
            public bool IsTransactional => unitOfWork.IsTransactional;
            public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => unitOfWork.BeginTransactionAsync(cancellationToken);
            public Task CompleteAsync(CancellationToken cancellationToken = default) => unitOfWork.CompleteAsync(cancellationToken);
            public IRepository<T> Repository<T>() where T : class, IEntity => unitOfWork.Repository<T>();
            public ValueTask DisposeAsync()
            {
                DataStore.Clear();
                return unitOfWork.DisposeAsync();
            }
        }
    }
}
