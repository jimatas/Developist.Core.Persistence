using System.Linq.Expressions;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for the <see cref="IRepository{T}"/> interface.
/// </summary>
public static partial class RepositoryExtensions
{
    /// <summary>
    /// Returns a paginated list of entities from the repository, using the provided paginator configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="configurePaginator">The action used to configure the paginator.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> ListAsync<T>(this IRepository<T> repository,
        Action<SortingPaginator<T>> configurePaginator,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurePaginator);

        var paginator = new SortingPaginator<T>();
        configurePaginator(paginator);

        return repository.ListAsync(paginator, cancellationToken);
    }

    /// <summary>
    /// Returns a paginated list of entities from the repository that satisfy a predicate, using the provided paginator configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="configurePaginator">The action used to configure the paginator.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> ListAsync<T>(this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        Action<SortingPaginator<T>> configurePaginator,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurePaginator);

        var paginator = new SortingPaginator<T>();
        configurePaginator(paginator);

        return repository.ListAsync(new PredicateFilterCriteria<T>(predicate), paginator, cancellationToken);
    }
}
