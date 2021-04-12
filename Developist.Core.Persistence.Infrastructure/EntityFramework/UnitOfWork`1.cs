// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
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

        public UnitOfWork(TDbContext dbContext, IRepositoryFactory<TDbContext> repositoryFactory, ILogger<UnitOfWork<TDbContext>> logger)
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
            }
            catch (Exception exception)
            {
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
                await DbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception exception)
            {
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

        protected override void ReleaseManagedResources()
        {
            repositories.Clear();
            DbContext.Dispose();

            base.ReleaseManagedResources();
        }

        protected async override ValueTask ReleaseManagedResourcesAsync()
        {
            repositories.Clear();
            await DbContext.DisposeAsync().ConfigureAwait(false);
            
            await base.ReleaseManagedResourcesAsync().ConfigureAwait(false);
        }
    }
}
