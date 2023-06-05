namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for the <see cref="IRepository{T}"/> interface.
/// </summary>
public static partial class RepositoryExtensions
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
        if (repository is not Repository<T> efRepository)
        {
            throw new NotSupportedException($"The repository of type '{repository.GetType().Name}' does not have support for including related entities.");
        }

        ArgumentNullException.ThrowIfNull(configureIncludePaths);

        var includePaths = new IncludePathsBuilder<T>();
        configureIncludePaths(includePaths);

        IQueryBuilder<T> queryBuilder = new IncludableQueryBuilder<T>(includePaths);
        if (efRepository is QueryBuildingRepository<T> { QueryBuilder: { } existingQueryBuilder })
        {
            queryBuilder = new CompositeQueryBuilder<T>(existingQueryBuilder, queryBuilder);
        }

        return new QueryBuildingRepository<T>(efRepository.UnitOfWork, queryBuilder);
    }

    /// <summary>
    /// Creates a new instance of the repository that performs queries without tracking entity changes.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <returns>A new repository instance with tracking disabled.</returns>
    public static IRepository<T> WithoutTracking<T>(this IRepository<T> repository) where T : class
    {
        if (repository is not Repository<T> efRepository)
        {
            throw new NotSupportedException($"The repository of type '{repository.GetType().Name}' does not have support for tracking entities.");
        }

        IQueryBuilder<T> queryBuilder = new NonTrackingQueryBuilder<T>();
        if (efRepository is QueryBuildingRepository<T> { QueryBuilder: { } existingQueryBuilder })
        {
            queryBuilder = new CompositeQueryBuilder<T>(existingQueryBuilder, queryBuilder);
        }

        return new QueryBuildingRepository<T>(efRepository.UnitOfWork, queryBuilder);
    }
}
