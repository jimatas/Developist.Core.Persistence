using Developist.Core.Persistence;
using Developist.Core.Persistence.Pagination;

namespace Developist.Customers.Persistence;

/// <summary>
/// Serves as a foundational base for implementing the Specification pattern through the <see cref="ICriteria{T}"/> abstraction.
/// </summary>
/// <typeparam name="T">The type of the items being paginated and filtered.</typeparam>
public abstract class PaginatedFilterCriteriaBase<T> : PaginationCriteria<T>, IFilterCriteria<T>
{
    private string? _sortString;
    private bool _isFiltered;

    /// <summary>
    /// Gets or sets the sorting criteria expressed as a string. The string format is parsed and applied to the query.
    /// </summary>
    /// <remarks>
    /// The supported format is <c>Field1 [ASC|DESC][, Field2 [ASC|DESC]]</c>. Here, "Field1", "Field2", etc., are property names.
    /// The "ASC" (ascending) or "DESC" (descending) sorting direction is optional, with "ASC" being the default if not specified.
    /// </remarks>
    /// <value>A value of <see langword="null"/> or whitespace clears the current sort criteria.</value>
    public new string? SortCriteria
    {
        get => _sortString;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                base.SortCriteria.Clear();
                _sortString = null; // Normalize to null.
            }
            else
            {
                ParseSortString(value);
                _sortString = value;
            }
        }
    }

    /// <summary>
    /// Applies either filter criteria or pagination criteria to the given query based on whether filter criteria have already been applied.
    /// If filter criteria have already been applied, it applies pagination criteria, and vice versa.
    /// </summary>
    /// <param name="query">The query to which filter or pagination criteria are to be applied.</param>
    /// <returns>The <see cref="IQueryable{T}"/> after applying either filter or pagination criteria.</returns>
    public new IQueryable<T> Apply(IQueryable<T> query)
    {
        if (_isFiltered)
        {
            query = base.Apply(query);
            _isFiltered = false;
        }
        else
        {
            query = ApplyFilter(query);
            _isFiltered = true;
        }

        return query;
    }

    /// <summary>
    /// When overridden in a derived class, applies custom filter criteria to the query.
    /// </summary>
    /// <param name="query">The query to which the filter criteria are to be applied.</param>
    /// <returns>The <see cref="IQueryable{T}"/> after applying the filter criteria.</returns>
    protected abstract IQueryable<T> ApplyFilter(IQueryable<T> query);

    /// <summary>
    /// Parses the provided sort string and updates the sorting criteria of the current instance.
    /// </summary>
    /// <param name="sortString">The sort string to be parsed into individual <see cref="SortCriterion{T}"/> objects.</param>
    protected void ParseSortString(string sortString)
    {
        new PaginationCriteriaConfigurator<T>(this).SortByString(sortString);
    }
}
