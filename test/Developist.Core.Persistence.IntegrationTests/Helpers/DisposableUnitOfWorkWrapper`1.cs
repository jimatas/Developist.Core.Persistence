using Developist.Core.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.IntegrationTests.Helpers;

public sealed class DisposableUnitOfWorkWrapper<TContext> : IUnitOfWork<TContext>, IAsyncDisposable
    where TContext : DbContext
{
    private readonly IUnitOfWork<TContext> _wrappedUnitOfWork;
    private readonly Func<DisposableUnitOfWorkWrapper<TContext>, ValueTask> _disposeAction;

    public DisposableUnitOfWorkWrapper(
        IUnitOfWork<TContext> unitOfWork,
        Func<DisposableUnitOfWorkWrapper<TContext>, ValueTask>? disposeAction = default)
    {
        _wrappedUnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _disposeAction = disposeAction ?? (_ => default);
    }

    public TContext DbContext => _wrappedUnitOfWork.DbContext;

    DbContext IUnitOfWorkBase.DbContext => DbContext;

    public event EventHandler<UnitOfWorkCompletedEventArgs>? Completed
    {
        add => _wrappedUnitOfWork.Completed += value;
        remove => _wrappedUnitOfWork.Completed -= value;
    }

    public bool HasActiveTransaction => _wrappedUnitOfWork.HasActiveTransaction;

    public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        return _wrappedUnitOfWork.BeginTransactionAsync(cancellationToken);
    }

    public Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        return _wrappedUnitOfWork.CompleteAsync(cancellationToken);
    }

    public IRepository<T> Repository<T>() where T : class
    {
        return _wrappedUnitOfWork.Repository<T>();
    }

    public async ValueTask DisposeAsync()
    {
        await _disposeAction(this).ConfigureAwait(false);
    }
}
