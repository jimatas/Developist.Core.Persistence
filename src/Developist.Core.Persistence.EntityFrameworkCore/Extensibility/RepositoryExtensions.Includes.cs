using Developist.Core.ArgumentValidation;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <inheritdoc/>
public static partial class RepositoryExtensions
{
    /// <summary>
    /// Creates a new instance of the repository with support for including related entities, based on the specified include paths configuration.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <param name="configureIncludes">An action to configure the include paths for eager loading related entities.</param>
    /// <returns>A new repository instance with eager loading capabilities.</returns>
    public static IRepository<T> WithIncludes<T>(this IRepository<T> repository,
        Action<IIncludesBuilder<T>> configureIncludes) where T : class
    {
        var includes = new IncludesBuilder<T>();
        configureIncludes(includes);

        return repository.WithIncludes(includes);
    }

    /// <summary>
    /// Creates a new instance of the repository with support for including related entities, based on the specified include paths.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <param name="includes">The include paths for eager loading related entities.</param>
    /// <returns>A new repository instance with eager loading capabilities.</returns>
    public static IRepository<T> WithIncludes<T>(this IRepository<T> repository, IIncludesBuilder<T> includes) where T : class
    {
        Ensure.Argument.NotNull(includes);

        return repository.ExtendWithFeature(
            queryExtender: new IncludableQueryExtender<T>(includes),
            featureName: "including related entities");
    }

    /// <summary>
    /// Returns a new includes builder to configure paths for eager loading of related entities.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="_">The repository instance, unused.</param>
    /// <returns>A new instance of <see cref="IIncludesBuilder{T}"/>.</returns>
    public static IIncludesBuilder<T> BuildIncludes<T>(this IRepository<T> _)
        where T : class => new IncludesBuilder<T>();

    internal class IncludableQueryExtender<T> : IQueryExtender<T> where T : class
    {
        private readonly IIncludesBuilder<T> _includes;

        public IncludableQueryExtender(IIncludesBuilder<T> includes) => _includes = includes;

        public IQueryable<T> Extend(IQueryable<T> query)
        {
            foreach (var path in _includes.AsList().Distinct())
            {
                query = query.Include(path);
            }

            return query;
        }
    }
}
