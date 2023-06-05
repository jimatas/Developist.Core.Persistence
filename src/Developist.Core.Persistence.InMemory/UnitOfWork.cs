using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    /// <summary>
    /// Represents an in-memory implementation of the <see cref="IUnitOfWork"/> interface that does not support transactions.
    /// </summary>
    public class UnitOfWork : UnitOfWorkBase, IUnitOfWork
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="repositoryFactory">The repository factory to use for creating repositories.</param>
        public UnitOfWork(IRepositoryFactory repositoryFactory)
            : base(repositoryFactory) { }

        /// <inheritdoc/>
        public override bool HasActiveTransaction => false;

        /// <inheritdoc/>
        public override Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <inheritdoc/>
        public override Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            OnCompleted(new UnitOfWorkCompletedEventArgs(this));

            return Task.CompletedTask;
        }
    }
}
