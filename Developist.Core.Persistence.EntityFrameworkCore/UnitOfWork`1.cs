using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

using System.Diagnostics.CodeAnalysis;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public class UnitOfWork<TDbContext> : UnitOfWorkBase, IUnitOfWork<TDbContext>
        where TDbContext : DbContext, new()
    {
        private IDbContextTransaction? dbContextTransaction;
        private readonly bool disposeOfDbContext;

        public UnitOfWork(TDbContext? dbContext = null, IRepositoryFactory<TDbContext>? repositoryFactory = null)
            : base(repositoryFactory ?? new RepositoryFactory<TDbContext>())
        {
            if (dbContext is null)
            {
                dbContext = new TDbContext();
                disposeOfDbContext = true;
            }
            DbContext = dbContext;
        }

        public TDbContext DbContext { get; }

        [MemberNotNullWhen(true, nameof(dbContextTransaction))]
        public override bool IsTransactional => dbContextTransaction is not null;

        public override async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (IsTransactional)
            {
                throw new InvalidOperationException("An active transaction is already in progress. Nested transactions are not supported.");
            }
            dbContextTransaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
        }

        public override async Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                DbContext.ValidateChangedEntities();

                await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
                await CommitTransactionAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken).ConfigureAwait(false);
                throw;
            }

            OnCompleted(new UnitOfWorkCompletedEventArgs(this));
        }

        protected async Task CommitTransactionAsync(CancellationToken cancellationToken)
        {
            if (IsTransactional)
            {
                await dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
                await dbContextTransaction.DisposeAsync().ConfigureAwait(false);

                dbContextTransaction = null;
            }
        }

        protected async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (IsTransactional)
            {
                await dbContextTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
                await dbContextTransaction.DisposeAsync().ConfigureAwait(false);

                dbContextTransaction = null;
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            if (!IsDisposed)
            {
                await RollbackTransactionAsync().ConfigureAwait(false);
                if (disposeOfDbContext)
                {
                    await DbContext.DisposeAsync().ConfigureAwait(false);
                }
            }
            await base.DisposeAsyncCore().ConfigureAwait(false);
        }
    }
}
