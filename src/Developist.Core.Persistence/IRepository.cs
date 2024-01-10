namespace Developist.Core.Persistence;

/// <summary>
/// Represents a repository for entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IRepository<T> where T : class
{
    /// <summary>
    /// Gets the unit of work associated with this repository.
    /// </summary>
    IUnitOfWorkBase UnitOfWork { get; }

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
    /// Checks if any entity exists in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is a boolean indicating if any entity exists.</returns>
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any entity that matches the given filter criteria exists in the repository.
    /// </summary>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is a boolean indicating if any entity matching the criteria exists.</returns>
    Task<bool> AnyAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities in the repository.
    /// </summary>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is the number of entities in the repository.</returns>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Counts the number of entities in the repository that satisfy the given filter criteria.
    /// </summary>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is the number of entities in the repository that satisfy the given filter criteria.</returns>
    Task<int> CountAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the first entity from the repository.
    /// </summary>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is the first entity in the repository, or <see langword="null"/> if the repository is empty.</returns>
    Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves the first entity from the repository that satisfies the given filter criteria.
    /// </summary>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is the first entity that satisfies the filter criteria, or <see langword="null"/> if no entity is found.</returns>
    Task<T?> FirstOrDefaultAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a single entity from the repository that satisfies the given filter criteria, throwing an exception if multiple entities match the filter criteria.
    /// </summary>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is the single entity that satisfies the filter criteria, or <see langword="null"/> if no entity is found.</returns>
    Task<T?> SingleOrDefaultAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated list of entities from the repository.
    /// </summary>
    /// <param name="paginationCriteria">The pagination criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is a paginated list of entities from the repository.</returns>
    Task<IPaginatedList<T>> ListAsync(IPaginationCriteria<T> paginationCriteria, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paginated list of entities from the repository that satisfy the given filter criteria.
    /// </summary>
    /// <param name="filterCriteria">The filter criteria.</param>
    /// <param name="paginationCriteria">The pagination criteria.</param>
    /// <param name="cancellationToken">A token for canceling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.
    /// The result of the task is a paginated list of entities from the repository that satisfy the given filter criteria.</returns>
    Task<IPaginatedList<T>> ListAsync(IFilterCriteria<T> filterCriteria, IPaginationCriteria<T> paginationCriteria, CancellationToken cancellationToken = default);
}
