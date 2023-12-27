using Developist.Core.ArgumentValidation;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides an Entity Framework Core specific implementation of the <see cref="IUnitOfWorkBase"/> interface.
/// </summary>
/// <typeparam name="TContext">The type of the database context associated with the unit of work.</typeparam>
public class UnitOfWork<TContext> : IUnitOfWork<TContext>
    where TContext : DbContext
{
    private readonly ConcurrentDictionary<Type, object> _repositories = new();
    private readonly IRepositoryFactory<TContext> _repositoryFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWork{TContext}"/> class with the specified database context and repository factory.
    /// </summary>
    /// <param name="dbContext">The database context to use for this unit of work.</param>
    /// <param name="repositoryFactory">The repository factory to use for creating repository instances.</param>
    public UnitOfWork(TContext dbContext, IRepositoryFactory<TContext> repositoryFactory)
    {
        DbContext = Ensure.Argument.NotNull(dbContext);
        _repositoryFactory = Ensure.Argument.NotNull(repositoryFactory);
    }

    /// <inheritdoc/>
    public event EventHandler<UnitOfWorkCompletedEventArgs>? Completed;

    /// <inheritdoc/>
    public TContext DbContext { get; }

    /// <inheritdoc/>
    DbContext IUnitOfWork.DbContext => DbContext;

    /// <inheritdoc/>
    public async Task CompleteAsync(CancellationToken cancellationToken = default)
    {
        await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        Completed?.Invoke(this, new UnitOfWorkCompletedEventArgs(this));
    }

    /// <inheritdoc/>
    public IRepository<T> Repository<T>() where T : class
    {
        return (IRepository<T>)_repositories.GetOrAdd(typeof(T),
            _ => _repositoryFactory.Create<T>(this));
    }
}
