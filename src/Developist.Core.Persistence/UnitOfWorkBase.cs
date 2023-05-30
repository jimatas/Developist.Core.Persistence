using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// An abstract base class for implementing a unit of work pattern, which is responsible for managing transactions and repositories.
    /// </summary>
    public abstract class UnitOfWorkBase : IUnitOfWork
    {
        private readonly ConcurrentDictionary<Type, RepositoryHolder> _repositories;
        private readonly IRepositoryFactory _repositoryFactory;

        /// <summary>
        /// Constructs a new instance of the <see cref="UnitOfWorkBase"/> class using the specified repository factory.
        /// </summary>
        /// <param name="repositoryFactory">The factory used to create repositories.</param>
        protected UnitOfWorkBase(IRepositoryFactory repositoryFactory)
        {
            _repositories = new ConcurrentDictionary<Type, RepositoryHolder>();
            _repositoryFactory = repositoryFactory ?? throw new ArgumentNullException(nameof(repositoryFactory));
        }

        /// <inheritdoc/>
        public event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        /// <inheritdoc/>
        public abstract bool HasActiveTransaction { get; }

        /// <inheritdoc/>
        public abstract Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public abstract Task CompleteAsync(CancellationToken cancellationToken = default);

        /// <inheritdoc/>
        public virtual IRepository<T> Repository<T>() where T : class
        {
            return _repositories.GetOrAdd(typeof(T), _ => new RepositoryHolder(_repositoryFactory.Create<T>(this))).Repository<T>();
        }

        /// <summary>
        /// Raises the <see cref="Completed"/> event with the specified arguments.
        /// </summary>
        /// <param name="e">The <see cref="UnitOfWorkCompletedEventArgs"/> instance containing the event data.</param>
        /// <remarks>
        /// This method is called internally to raise the <see cref="Completed"/> event.
        /// </remarks>
        protected void OnCompleted(UnitOfWorkCompletedEventArgs e) => Completed?.Invoke(this, e);

        private readonly struct RepositoryHolder
        {
            private readonly object _repository;

            public RepositoryHolder(object repository) => _repository = repository;

            public IRepository<T> Repository<T>()
            {
                return (IRepository<T>)_repository;
            }
        }
    }
}
