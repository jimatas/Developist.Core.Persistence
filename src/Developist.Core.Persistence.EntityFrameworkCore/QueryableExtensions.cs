using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> objects.
/// </summary>
internal static class QueryableExtensions
{
    /// <summary>
    /// Includes related entities specified in <paramref name="includePathsBuilder"/> in the query results.
    /// </summary>
    /// <typeparam name="T">The type of entity being queried.</typeparam>
    /// <param name="query">The query to include related entities in.</param>
    /// <param name="includePathsBuilder">The <see cref="IIncludePathsBuilder{T}"/> that specifies the related entities to include.</param>
    /// <returns>The query with related entities included.</returns>
    public static IQueryable<T> WithIncludes<T>(
        this IQueryable<T> query,
        IIncludePathsBuilder<T> includePathsBuilder) where T : class
    {
        ArgumentNullException.ThrowIfNull(includePathsBuilder);

        return includePathsBuilder.AsList()
            .Distinct()
            .Aggregate(query, (query, path) => query.Include(path));
    }
}
