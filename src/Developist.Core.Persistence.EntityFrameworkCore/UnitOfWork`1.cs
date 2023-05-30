using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Diagnostics.CodeAnalysis;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides an Entity Framework Core specific implementation of the <see cref="IUnitOfWork"/> interface.
/// </summary>
public class UnitOfWork<TContext> : UnitOfWorkBase, IUnitOfWork<TContext>
    where TContext : DbContext
{
    private IDbContextTransaction? _dbContextTransaction;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class with the specified database context and repository factory.
    /// </summary>
    /// <param name="dbContext">The database context to use for this unit of work.</param>
    /// <param name="repositoryFactory">The repository factory to use for creating repository instances.</param>
    public UnitOfWork(TContext dbContext, IRepositoryFactory repositoryFactory)
        : base(repositoryFactory)
    {
        DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    /// <inheritdoc/>
    public TContext DbContext { get; }

    /// <inheritdoc/>
    [MemberNotNullWhen(true, nameof(_dbContextTransaction))]
    public override bool HasActiveTransaction => _dbContextTransaction is not null;

    /// <inheritdoc/>
    public override async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (HasActiveTransaction)
        {
            throw new InvalidOperationException("An active transaction is already in progress. "
                + "This operation does not support nested transactions. "
                + "Please complete the current unit of work before beginning a new transaction.");
        }

        _dbContextTransaction = await DbContext.Database.BeginTransactionAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
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

    /// <summary>
    /// Commits the current transaction and disposes the associated <see cref="IDbContextTransaction"/> object, if one is active.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        if (HasActiveTransaction)
        {
            await _dbContextTransaction.CommitAsync(cancellationToken).ConfigureAwait(false);
            await _dbContextTransaction.DisposeAsync().ConfigureAwait(false);
            _dbContextTransaction = null;
        }
    }

    /// <summary>
    /// Rolls back the current transaction and disposes the associated <see cref="IDbContextTransaction"/> object, if one is active.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (HasActiveTransaction)
        {
            await _dbContextTransaction.RollbackAsync(cancellationToken).ConfigureAwait(false);
            await _dbContextTransaction.DisposeAsync().ConfigureAwait(false);
            _dbContextTransaction = null;
        }
    }
}
