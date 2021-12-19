// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.EntityFramework
{
    public class UnitOfWork<TDbContext> : UnitOfWorkBase, IUnitOfWork<TDbContext>
        where TDbContext : DbContext, new()
    {
        private DbContextTransaction dbContextTransaction;
        private readonly bool disposeOfDbContext;

        public UnitOfWork() : this(new RepositoryFactory<TDbContext>()) { }
        public UnitOfWork(IRepositoryFactory<TDbContext> repositoryFactory)
            : this(new TDbContext(), repositoryFactory) => disposeOfDbContext = true;

        public UnitOfWork(TDbContext dbContext) : this(dbContext, new RepositoryFactory<TDbContext>()) { }
        public UnitOfWork(TDbContext dbContext, IRepositoryFactory<TDbContext> repositoryFactory)
            : base(repositoryFactory) => DbContext = Ensure.Argument.NotNull(dbContext, nameof(dbContext));

        public TDbContext DbContext { get; }
        public override bool IsTransactional => dbContextTransaction != null;

        public override void Complete()
        {
            try
            {
                DbContext.SaveChanges();
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            OnCompleted(new UnitOfWorkCompletedEventArgs(this));
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                CommitTransaction();
            }
            catch
            {
                RollbackTransaction();
                throw;
            }
            OnCompleted(new UnitOfWorkCompletedEventArgs(this));
        }

        public override void BeginTransaction()
        {
            if (IsTransactional)
            {
                throw new InvalidOperationException("An active transaction is already in progress. Nested transactions are not supported.");
            }
            dbContextTransaction = DbContext.Database.BeginTransaction();
        }

        protected void CommitTransaction()
        {
            if (IsTransactional)
            {
                dbContextTransaction.Commit();
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }

        protected void RollbackTransaction()
        {
            if (IsTransactional)
            {
                dbContextTransaction.Rollback();
                dbContextTransaction.Dispose();
                dbContextTransaction = null;
            }
        }

        protected override void ReleaseManagedResources()
        {
            RollbackTransaction();
            if (disposeOfDbContext)
            {
                DbContext.Dispose();
            }
            base.ReleaseManagedResources();
        }
    }
}
