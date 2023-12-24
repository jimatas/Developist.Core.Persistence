namespace Developist.Core.Persistence;

/// <summary>
/// Represents the base unit of work that manages a transaction and provides access to repositories.
/// </summary>
public interface IUnitOfWorkBase
{
    /// <summary>
    /// Occurs when the unit of work has been successfully completed.
    /// </summary>
    event EventHandler<UnitOfWorkCompletedEventArgs>? Completed;

    /// <summary>
    /// Gets the repository for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    /// <returns>The repository for entities of type <typeparamref name="T"/>.</returns>
    IRepository<T> Repository<T>() where T : class;

    /// <summary>
    /// Completes the unit of work and any pending transaction asynchronously.
    /// </summary>
    /// <param name="cancellationToken">A token for cancelling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CompleteAsync(CancellationToken cancellationToken = default);
}
