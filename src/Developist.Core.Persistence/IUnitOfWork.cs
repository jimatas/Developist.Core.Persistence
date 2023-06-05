using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Represents a unit of work that manages a transaction and provides access to repositories.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Occurs when the unit of work has completed its work.
        /// </summary>
        event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        /// <summary>
        /// Gets a value indicating whether there is an active transaction.
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Begins a transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Completes the unit of work and any pending transaction asynchronously.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CompleteAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the repository for entities of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>The repository for entities of type <typeparamref name="T"/>.</returns>
        IRepository<T> Repository<T>() where T : class;
    }
}
