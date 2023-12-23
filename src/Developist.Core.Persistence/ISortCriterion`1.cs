namespace Developist.Core.Persistence;

/// <summary>
/// Represents a sorting criterion for items of type <typeparamref name="T"/>, including a lambda expression as the key to sort by.
/// </summary>
/// <typeparam name="T">The type of the items to sort.</typeparam>
public interface ISortCriterion<T> : ISortCriterion
{
    /// <summary>
    /// Gets a lambda expression representing the property name on which to sort.
    /// </summary>
    new LambdaExpression Key { get; }

    /// <summary>
    /// Applies the sorting criterion to a query.
    /// </summary>
    /// <param name="query">The query to apply the sorting to.</param>
    /// <returns>The sorted query.</returns>
    IOrderedQueryable<T> Apply(IQueryable<T> query);

    /// <summary>
    /// Applies the sorting criterion to an already sorted query.
    /// </summary>
    /// <param name="sortedQuery">The already sorted query to apply additional sorting to.</param>
    /// <returns>The additionally sorted query.</returns>
    IOrderedQueryable<T> Apply(IOrderedQueryable<T> sortedQuery);
}
