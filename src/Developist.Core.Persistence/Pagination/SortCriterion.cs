using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence;

/// <summary>
/// Represents a sorting criterion for a query, encapsulating a sorting key and direction.
/// </summary>
public class SortCriterion : ISortCriterion
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SortCriterion"/> class with the specified sorting key and direction.
    /// </summary>
    /// <param name="key">The key used for sorting.</param>
    /// <param name="direction">The direction of the sort (ascending or descending).</param>
    protected SortCriterion(string key, SortDirection direction)
    {
        Key = Ensure.Argument.NotNullOrWhiteSpace(key);
        Direction = Ensure.Argument.NotInvalidEnum(direction);
    }

    /// <inheritdoc/>
    public string Key { get; }

    /// <inheritdoc/>
    public SortDirection Direction { get; }
}
