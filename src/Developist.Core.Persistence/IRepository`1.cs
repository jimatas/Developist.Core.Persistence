using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Represents a repository for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of entity.</typeparam>
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets the unit of work associated with this repository.
        /// </summary>
        IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Adds an entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        void Add(T entity);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Counts the number of entities in the repository.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the number of entities in the repository.</returns>
        Task<int> CountAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Counts the number of entities in the repository that satisfy the given filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the number of entities in the repository that satisfy the given filter criteria.</returns>
        Task<int> CountAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a single entity from the repository that satisfies the given filter criteria, throwing an exception if multiple entities match the filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the single entity that satisfies the filter criteria, or <c>null</c> if no entity is found.</returns>
        Task<T> SingleOrDefaultAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the first entity from the repository.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the first entity in the repository, or <c>null</c> if the repository is empty.</returns>
        Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves the first entity from the repository that satisfies the given filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is the first entity that satisfies the filter criteria, or <c>null</c> if no entity is found.</returns>
        Task<T> FirstOrDefaultAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Returns a paginated list of entities from the repository.
        /// </summary>
        /// <param name="paginator">The paginator.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is a paginated list of entities from the repository.</returns>
        Task<IPaginatedList<T>> ListAsync(IPaginator<T> paginator, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns a paginated list of entities from the repository that satisfy the given filter criteria.
        /// </summary>
        /// <param name="criteria">The filter criteria.</param>
        /// <param name="paginator">The paginator.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is a paginated list of entities from the repository that satisfy the given filter criteria.</returns>
        Task<IPaginatedList<T>> ListAsync(IFilterCriteria<T> criteria, IPaginator<T> paginator, CancellationToken cancellationToken = default);
    }
}
