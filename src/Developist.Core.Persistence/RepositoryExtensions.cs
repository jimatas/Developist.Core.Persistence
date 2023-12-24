using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.Pagination;

namespace Developist.Core.Persistence;

/// <summary>
/// Provides extension methods for <see cref="IRepository{T}"/> for common querying operations.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Checks whether any entity in the repository satisfies a specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to check entities in.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns <see langword="true"/> if any entity satisfies the predicate.</returns>
    public static Task<bool> AnyAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : class
    {
        return repository.AnyAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
    }

    /// <summary>
    /// Counts the number of entities in the repository that satisfy a predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to count entities from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns the number of entities that satisfy the predicate.</returns>
    public static Task<int> CountAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : class
    {
        return repository.CountAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
    }

    /// <summary>
    /// Retrieves the first entity from the repository that satisfies the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve the entity from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns the first entity that satisfies the predicate, or <see langword="null"/> if no entity is found.</returns>
    public static Task<T?> FirstOrDefaultAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : class
    {
        return repository.FirstOrDefaultAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
    }

    /// <summary>
    /// Retrieves a single entity from the repository that satisfies the specified predicate, throwing an exception if multiple entities match the predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve the entity from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns the single entity that satisfies the predicate, or <see langword="null"/> if no entity is found.</returns>
    public static Task<T?> SingleOrDefaultAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default) where T : class
    {
        return repository.SingleOrDefaultAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of entities from the repository, using the provided pagination criteria.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="configurePagination">The action used to configure the pagination criteria.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> ListAsync<T>(
        this IRepository<T> repository,
        Action<PaginationCriteriaConfigurator<T>> configurePagination,
        CancellationToken cancellationToken = default) where T : class
    {
        var paginationCriteria = new PaginationCriteria<T>();
        configurePagination(new PaginationCriteriaConfigurator<T>(paginationCriteria));

        return repository.ListAsync(paginationCriteria, cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of entities from the repository that satisfy a predicate, using the provided pagination criteria
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="configurePagination">The action used to configure the pagination criteria.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> ListAsync<T>(
        this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        Action<PaginationCriteriaConfigurator<T>> configurePagination,
        CancellationToken cancellationToken = default) where T : class
    {
        var paginationCriteria = new PaginationCriteria<T>();
        configurePagination(new PaginationCriteriaConfigurator<T>(paginationCriteria));

        return repository.ListAsync(new PredicateFilterCriteria<T>(predicate), paginationCriteria, cancellationToken);
    }
}
