namespace Developist.Core.Persistence;

/// <summary>
/// Defines a contract for applying criteria to a query.
/// </summary>
/// <typeparam name="T">The type of data the criteria applies to.</typeparam>
public interface ICriteria<T>
{
    /// <summary>
    /// Applies the criteria to a given query.
    /// </summary>
    /// <param name="query">The query to which the criteria should be applied.</param>
    /// <returns>The modified query with the criteria applied.</returns>
    IQueryable<T> Apply(IQueryable<T> query);
}
