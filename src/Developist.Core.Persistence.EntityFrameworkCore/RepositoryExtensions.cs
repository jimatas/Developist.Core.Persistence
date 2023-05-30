using Developist.Core.Persistence.EntityFrameworkCore.Pagination.Sorting;
using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.IncludePaths;
using Developist.Core.Persistence.Pagination;
using System.Linq.Expressions;

namespace Developist.Core.Persistence;

/// <summary>
/// Provides extension methods for the <see cref="IRepository{T}"/> interface.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Counts the number of entities in the repository that satisfy a predicate.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to count entities from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns the number of entities that satisfy the predicate.</returns>
    public static Task<int> CountAsync<T>(this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return repository.CountAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
    }

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
    /// Returns a paginated list of entities from the repository, using the provided paginator and include paths configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="configurePaginator">The action used to configure the paginator.</param>
    /// <param name="configureIncludePaths">The include paths used to include related entities in the query.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> ListAsync<T>(this IRepository<T> repository,
        Action<SortingPaginator<T>> configurePaginator,
        Action<IIncludePathsBuilder<T>> configureIncludePaths,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurePaginator);

        var paginator = new SortingPaginator<T>();
        configurePaginator(paginator);

        ArgumentNullException.ThrowIfNull(configureIncludePaths);

        var includePaths = new IncludePathsBuilder<T>();
        configureIncludePaths(includePaths);

        return repository.ListAsync(paginator, includePaths, cancellationToken);
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
    public static Task<IPaginatedList<T>> FindAsync<T>(this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        Action<SortingPaginator<T>> configurePaginator,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurePaginator);

        var paginator = new SortingPaginator<T>();
        configurePaginator(paginator);

        return repository.FindAsync(new PredicateFilterCriteria<T>(predicate), paginator, cancellationToken);
    }

    /// <summary>
    /// Retrieves a paginated list of entities from the repository that satisfy a predicate, using the provided paginator and include paths configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The repository to retrieve entities from.</param>
    /// <param name="predicate">The predicate used to filter entities.</param>
    /// <param name="configurePaginator">The action used to configure the paginator.</param>
    /// <param name="configureIncludePaths">The action used to configure the include paths of related entities.</param>
    /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns a paginated list of entities.</returns>
    public static Task<IPaginatedList<T>> FindAsync<T>(this IRepository<T> repository,
        Expression<Func<T, bool>> predicate,
        Action<SortingPaginator<T>> configurePaginator,
        Action<IIncludePathsBuilder<T>> configureIncludePaths,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(configurePaginator);

        var paginator = new SortingPaginator<T>();
        configurePaginator(paginator);

        ArgumentNullException.ThrowIfNull(configureIncludePaths);

        var includePaths = new IncludePathsBuilder<T>();
        configureIncludePaths(includePaths);

        return repository.FindAsync(new PredicateFilterCriteria<T>(predicate), paginator, includePaths, cancellationToken);
    }
}
