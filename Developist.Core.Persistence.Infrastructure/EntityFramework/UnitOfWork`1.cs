// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.EntityFramework
{
    public class UnitOfWork<TDbContext> : DisposableBase, IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly ConcurrentDictionary<Type, RepositoryWrapper> repositories = new();
        private readonly IRepositoryFactory<TDbContext> repositoryFactory;
        private readonly ILogger logger;
        private bool isTransactional;
        private IDbContextTransaction dbContextTransaction;

        public UnitOfWork(TDbContext dbContext, IRepositoryFactory<TDbContext> repositoryFactory, ILogger<UnitOfWork<TDbContext>> logger = null)
        {
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            this.logger = logger ?? NullLogger<UnitOfWork<TDbContext>>.Instance;
        }

        public TDbContext DbContext { get; }

        public event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        public virtual void Complete()
        {
            try
            {
                DbContext.ValidateChangedEntities();
                
                BeginTransaction();
                DbContext.SaveChanges();
                CommitTransaction();
            }
            catch (Exception exception)
            {
                RollbackTransaction();
                logger.LogWarning(exception, $"Exception thrown during {nameof(UnitOfWork<TDbContext>)}<{nameof(TDbContext)}>.{nameof(Complete)} call.");
                throw;
            }

            Completed?.Invoke(this, new UnitOfWorkCompletedEventArgs(this));
        }

        public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                DbContext.ValidateChangedEntities();

                await BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                await RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                logger.LogWarning(exception, $"Exception thrown during {nameof(UnitOfWork<TDbContext>)}<{nameof(TDbContext)}>.{nameof(CompleteAsync)} call.");
                throw;
            }

            Completed?.Invoke(this, new UnitOfWorkCompletedEventArgs(this));
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        {
            var wrapper = repositories.GetOrAdd(typeof(TEntity), _ => new(repositoryFactory.Create<TEntity>(this)));
            return wrapper.Repository<TEntity>();
        }

        public void EnsureTransactional() => isTransactional = true;

        protected override void ReleaseManagedResources()
        {
            repositories.Clear();
            RollbackTransaction();
            base.ReleaseManagedResources();
        }

        protected async override ValueTask ReleaseManagedResourcesAsync()
        {
            repositories.Clear();
            await RollbackTransactionAsync().ConfigureAwait(false);
            await base.ReleaseManagedResourcesAsync().ConfigureAwait(false);
        }

        #region Transaction management
        private void BeginTransaction()
        {
            if (isTransactional)
            {
                dbContextTransaction = DbContext.Database.BeginTransaction();
            }
        }

        private void CommitTransaction()
        {
            if (isTransactional && dbContextTransaction is not null)
            {
                dbContextTransaction.Commit();
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }

        private void RollbackTransaction()
        {
            if (isTransactional && dbContextTransaction is not null)
            {
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }

        private async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (isTransactional)
            {
                dbContextTransaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (isTransactional && dbContextTransaction is not null)
            {
                await dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                await dbContextTransaction.DisposeAsync().ConfigureAwait(false);
                dbContextTransaction = null;
            }
        }

        private async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (isTransactional && dbContextTransaction is not null)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                await dbContextTransaction.DisposeAsync().ConfigureAwait(false);
                dbContextTransaction = null;
            }
        }
        #endregion
    }
}
