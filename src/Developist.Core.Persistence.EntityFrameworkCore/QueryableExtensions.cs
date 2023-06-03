using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> objects.
/// </summary>
internal static class QueryableExtensions
{
    /// <summary>
    /// Includes related entities specified in <paramref name="includePaths"/> in the query results.
    /// </summary>
    /// <typeparam name="T">The type of entity being queried.</typeparam>
    /// <param name="query">The query to include related entities in.</param>
    /// <param name="includePaths">The <see cref="IIncludePathsBuilder{T}"/> that specifies the related entities to include.</param>
    /// <returns>The query with related entities included.</returns>
    public static IQueryable<T> WithIncludes<T>(
        this IQueryable<T> query,
        IIncludePathsBuilder<T> includePaths) where T : class
    {
        ArgumentNullException.ThrowIfNull(includePaths);

        return includePaths.AsList()
            .Distinct()
            .Aggregate(query, (query, path) => query.Include(path));
    }
}
