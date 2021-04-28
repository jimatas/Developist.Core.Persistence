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
        private readonly SemaphoreLocker semaphore = new();
        private readonly ConcurrentDictionary<Type, RepositoryWrapper> repositories = new();
        private readonly IRepositoryFactory<TDbContext> repositoryFactory;
        private readonly ILogger logger;
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

        #region Transaction management
        public virtual void BeginTransaction()
        {
            semaphore.Lock(() =>
            {
                if (dbContextTransaction is not null)
                {
                    throw new InvalidOperationException("An active transaction is already in progress. Nested transactions are not supported.");
                }
                dbContextTransaction = DbContext.Database.BeginTransaction();
            });
        }

        public async virtual Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            await semaphore.LockAsync(async () =>
            {
                if (dbContextTransaction is not null)
                {
                    throw new InvalidOperationException("An active transaction is already in progress. Nested transactions are not supported.");
                }
                dbContextTransaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
            });
        }

        protected void CommitTransaction()
        {
            semaphore.Lock(() =>
            {
                if (dbContextTransaction is not null)
                {
                    dbContextTransaction.Commit();
                    dbContextTransaction.Dispose();
                    dbContextTransaction = null;
                }
            });
        }

        protected void RollbackTransaction()
        {
            semaphore.Lock(() =>
            {
                if (dbContextTransaction is not null)
                {
                    dbContextTransaction.Rollback();
                    dbContextTransaction.Dispose();
                    dbContextTransaction = null;
                }
            });
        }

        protected async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            await semaphore.LockAsync(async () =>
            {
                if (dbContextTransaction is not null)
                {
                    await dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                    await dbContextTransaction.DisposeAsync().ConfigureAwait(false);
                    dbContextTransaction = null;
                }
            });
        }

        protected async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            await semaphore.LockAsync(async () =>
            {
                if (dbContextTransaction is not null)
                {
                    await dbContextTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                    await dbContextTransaction.DisposeAsync().ConfigureAwait(false);
                    dbContextTransaction = null;
                }
            });
        }
        #endregion

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
    }
}
