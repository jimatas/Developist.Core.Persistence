namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Represents a strongly-typed sorting criterion for a query.
/// </summary>
/// <typeparam name="T">The type of the items to sort.</typeparam>
public class SortCriterion<T> : SortCriterion, ISortCriterion<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortCriterion{T}"/> class with the specified sorting key and direction.
    /// </summary>
    /// <param name="key">The key used for sorting.</param>
    /// <param name="direction">The direction of the sort (ascending or descending).</param>
    public SortCriterion(string key, SortDirection direction)
        : base(key, direction)
    {
        Key = key.GetPropertySelector<T>();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SortCriterion{T}"/> class with the specified lambda expression representing the sorting key and direction.
    /// </summary>
    /// <param name="key">A lambda expression representing the key used for sorting.</param>
    /// <param name="direction">The direction of the sort (ascending or descending).</param>
    protected SortCriterion(LambdaExpression key, SortDirection direction)
        : base(key.GetPropertyName()
              ?? throw new ArgumentException(
                  message: "The provided lambda expression must represent a property.",
                  paramName: nameof(key)),
            direction)
    {
        Key = key;
    }

    /// <inheritdoc/>
    public new LambdaExpression Key { get; }

    /// <inheritdoc/>
    public virtual IOrderedQueryable<T> Apply(IQueryable<T> query)
    {
        var method = GetSortMethod(initial: true);

        return (IOrderedQueryable<T>)method.Invoke(null, new object[] { query, Key })!;
    }

    /// <inheritdoc/>
    public virtual IOrderedQueryable<T> Apply(IOrderedQueryable<T> sortedQuery)
    {
        var method = GetSortMethod(initial: false);

        return (IOrderedQueryable<T>)method.Invoke(null, new object[] { sortedQuery, Key })!;
    }

    private MethodInfo GetSortMethod(bool initial)
    {
        var methodName = GetSortMethodName(initial);

        return typeof(Queryable).GetMethods()
            .Single(method => method.Name.Equals(methodName) && method.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(T), Key.ReturnType);
    }

    private string GetSortMethodName(bool initial)
    {
        return initial
            ? Direction is SortDirection.Ascending
                ? nameof(Queryable.OrderBy)
                : nameof(Queryable.OrderByDescending)
            : Direction is SortDirection.Ascending
                ? nameof(Queryable.ThenBy)
                : nameof(Queryable.ThenByDescending);
    }
}
