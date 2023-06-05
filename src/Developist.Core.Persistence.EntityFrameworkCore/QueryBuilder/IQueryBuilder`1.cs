namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a query builder interface for extending or modifying queries with additional operations such as includes or filters.
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
internal interface IQueryBuilder<T>
{
    /// <summary>
    /// Builds and prepares a query by extending or modifying it with additional operations.
    /// </summary>
    /// <param name="query">The original query to be built upon.</param>
    /// <returns>The built and prepared query.</returns>
    IQueryable<T> BuildQuery(IQueryable<T> query);
}
