using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.Utilities;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.Tests.Integration
{
    public sealed class UnitOfWorkWrapper<TDbContext> : IUnitOfWork<TDbContext>
        where TDbContext : DbContext
    {
        private readonly IUnitOfWork<TDbContext> wrappedUnitOfWork;
        private readonly Func<UnitOfWorkWrapper<TDbContext>, ValueTask> cleanupAction;

        public UnitOfWorkWrapper(IUnitOfWork<TDbContext> unitOfWork, Func<UnitOfWorkWrapper<TDbContext>, ValueTask>? asyncCleanupAction = null)
        {
            wrappedUnitOfWork = ArgumentNullExceptionHelper.ThrowIfNull(() => unitOfWork);
            cleanupAction = asyncCleanupAction ?? new Func<UnitOfWorkWrapper<TDbContext>, ValueTask>(_ => default);
        }

        public TDbContext DbContext => wrappedUnitOfWork.DbContext;
        
        public event EventHandler<UnitOfWorkCompletedEventArgs>? Completed
        {
            add => wrappedUnitOfWork.Completed += value;
            remove => wrappedUnitOfWork.Completed -= value;
        }
        public Task CompleteAsync(CancellationToken cancellationToken = default) => wrappedUnitOfWork.CompleteAsync(cancellationToken);
        public bool IsTransactional => wrappedUnitOfWork.IsTransactional;
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) => wrappedUnitOfWork.BeginTransactionAsync(cancellationToken);
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity => wrappedUnitOfWork.Repository<TEntity>();

        public async ValueTask DisposeAsync()
        {
            await wrappedUnitOfWork.DisposeAsync().ConfigureAwait(false);
            await cleanupAction(this).ConfigureAwait(false);
        }
    }
}
