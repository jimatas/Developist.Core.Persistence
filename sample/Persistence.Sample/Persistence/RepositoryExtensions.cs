using Developist.Core.Persistence;

namespace Developist.Customers.Persistence;

/// <summary>
/// Provides extension methods for working with custom criteria in repositories.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Retrieves a paginated list of entities from the repository using custom filter and pagination criteria.
    /// </summary>
    /// <typeparam name="T">The type of entities to retrieve.</typeparam>
    /// <param name="repository">The repository from which to retrieve the entities.</param>
    /// <param name="criteria">The custom criteria for filtering and pagination.</param>
    /// <param name="cancellationToken">A token for cancelling the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation. The result of the task is a paginated list of entities from the repository.</returns>
    public static Task<IPaginatedList<T>> ListCustomAsync<T>(this IRepository<T> repository,
        PaginatedFilterCriteriaBase<T> criteria,
        CancellationToken cancellationToken = default) where T : class
    {
        return repository.ListAsync(criteria, criteria, cancellationToken);
    }
}
