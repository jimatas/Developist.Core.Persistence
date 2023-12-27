using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence.Filtering;

/// <summary>
/// Provides extension methods for <see cref="IQueryable{T}"/> to enable filtering.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Filters the query according to the specified filter criteria.
    /// </summary>
    /// <typeparam name="T">The type of the items in the query.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="filterCriteria">The filter criteria to apply.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the filtered query.</returns>
    public static IQueryable<T> Filter<T>(this IQueryable<T> query, IFilterCriteria<T> filterCriteria)
    {
        Ensure.Argument.NotNull(filterCriteria);

        return filterCriteria.Apply(query);
    }
}
