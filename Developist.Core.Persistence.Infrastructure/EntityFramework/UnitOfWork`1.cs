// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.EntityFramework
{
    public class UnitOfWork<TDbContext> : IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        private readonly ConcurrentDictionary<Type, RepositoryWrapper> repositories = new();
        private readonly IRepositoryFactory<TDbContext> repositoryFactory;

        public UnitOfWork(IRepositoryFactory<TDbContext> repositoryFactory, TDbContext dbContext)
        {
            this.repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
            DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public TDbContext DbContext { get; }

        public event EventHandler<UnitOfWorkEventArgs> Completed;

        public virtual void Complete()
        {
            DbContext.ValidateChangedEntities();
            DbContext.SaveChanges();

            Completed?.Invoke(this, new UnitOfWorkEventArgs(this));
        }

        public virtual async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            DbContext.ValidateChangedEntities();
            await DbContext.SaveChangesAsync(cancellationToken);

            Completed?.Invoke(this, new UnitOfWorkEventArgs(this));
        }

        public virtual IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity
        {
            var wrapper = repositories.GetOrAdd(typeof(TEntity), _ => new(repositoryFactory.Create<TEntity>(this)));
            return wrapper.Repository<TEntity>();
        }
    }
}
