using System.Linq.Expressions;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for the <see cref="IRepository{T}"/> interface.
/// </summary>
public static class RepositoryExtensions
{
    /// <summary>
    /// Creates a new instance of the repository with support for including related entities, based on the specified repository and include paths configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <param name="configureIncludePaths">An action to configure the include paths for eager loading related entities.</param>
    /// <returns>A new repository instance with eager loading capabilities.</returns>
    public static IRepository<T> WithIncludes<T>(this IRepository<T> repository,
        Action<IIncludePathsBuilder<T>> configureIncludePaths) where T : class
    {
        ArgumentNullException.ThrowIfNull(configureIncludePaths);

        var includePathsBuilder = new IncludePathsBuilder<T>();
        configureIncludePaths(includePathsBuilder);

        if (repository is not Repository<T> efRepository)
        {
            throw new NotSupportedException($"The repository of type '{repository.GetType().Name}' does not have support for including related entities.");
        }

        if (efRepository is RepositoryWithIncludes<T> efRepositoryWithIncludes)
        {
            foreach (var path in efRepositoryWithIncludes.IncludePathsBuilder.AsList())
            {
                includePathsBuilder.Include(path);
            }
        }

        return new RepositoryWithIncludes<T>(efRepository.UnitOfWork, includePathsBuilder);
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
