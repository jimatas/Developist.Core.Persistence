namespace Developist.Core.Persistence;

/// <summary>
/// Defines a single sorting criterion including the key to sort by and the sort direction.
/// </summary>
public interface ISortCriterion
{
    /// <summary>
    /// Gets the key or property name on which to sort.
    /// </summary>
    string Key { get; }

    /// <summary>
    /// Gets the direction (ascending or descending) for sorting.
    /// </summary>
    SortDirection Direction { get; }
}
