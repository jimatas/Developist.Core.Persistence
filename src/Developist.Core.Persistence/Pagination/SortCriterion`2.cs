using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Represents a sorting criterion for a query, providing a more specific sorting key and direction.
/// </summary>
/// <typeparam name="T">The type of the items to sort.</typeparam>
/// <typeparam name="TProperty">The type of the property by which the sorting is performed.</typeparam>
public class SortCriterion<T, TProperty> : SortCriterion<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortCriterion{T, TProperty}"/> class with the specified expression representing the sorting key and direction.
    /// </summary>
    /// <param name="key">An expression representing the key used for sorting.</param>
    /// <param name="direction">The direction of the sort (ascending or descending).</param>
    public SortCriterion(Expression<Func<T, TProperty>> key, SortDirection direction)
        : base(Ensure.NotNull(key), direction)
    {
        Key = key;
    }

    /// <summary>
    /// Gets a strongly-typed lambda expression representing the property of type <typeparamref name="TProperty"/> on which to sort.
    /// </summary>
    public new Expression<Func<T, TProperty>> Key { get; }

    /// <inheritdoc/>
    public override IOrderedQueryable<T> Apply(IQueryable<T> query)
    {
        return Direction is SortDirection.Ascending
            ? query.OrderBy(Key)
            : query.OrderByDescending(Key);
    }

    /// <inheritdoc/>
    public override IOrderedQueryable<T> Apply(IOrderedQueryable<T> sortedQuery)
    {
        return Direction is SortDirection.Ascending
            ? sortedQuery.ThenBy(Key)
            : sortedQuery.ThenByDescending(Key);
    }
}
