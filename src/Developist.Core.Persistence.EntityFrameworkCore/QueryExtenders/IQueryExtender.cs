namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a query builder interface for extending or modifying queries with additional operations such as includes or filters.
/// </summary>
/// <remarks>
/// This interface is for internal use, and its functionality is exposed externally through extension methods of the repository.
/// </remarks>
/// <typeparam name="T">The entity type being queried.</typeparam>
public interface IQueryExtender<T>
{
    /// <summary>
    /// Extends the provided query with additional operations.
    /// </summary>
    /// <param name="query">The query to be extended.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that represents the extended query.</returns>
    IQueryable<T> Extend(IQueryable<T> query);
}
