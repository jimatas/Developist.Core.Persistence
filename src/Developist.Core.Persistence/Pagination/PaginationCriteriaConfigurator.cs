using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Provides a fluent configuration interface for customizing a <see cref="PaginationCriteria{T}"/> instance.
/// </summary>
/// <typeparam name="T">The type of items to paginate.</typeparam>
public class PaginationCriteriaConfigurator<T>
{
    private readonly PaginationCriteria<T> _paginationCriteria;

    /// <summary>
    /// Initializes a new instance of the <see cref="PaginationCriteriaConfigurator{T}"/> class.
    /// </summary>
    /// <param name="paginationCriteria">The <see cref="PaginationCriteria{T}"/> instance to configure.</param>
    public PaginationCriteriaConfigurator(PaginationCriteria<T> paginationCriteria)
    {
        _paginationCriteria = Ensure.NotNull(paginationCriteria);
    }

    /// <summary>
    /// Sets the starting page number for the pagination criteria.
    /// </summary>
    /// <param name="pageNumber">The starting page number, with 1 representing the first page.</param>
    /// <returns>A reference to itself for method chaining.</returns>
    public PaginationCriteriaConfigurator<T> StartAtPage(int pageNumber)
    {
        _paginationCriteria.PageNumber = pageNumber;

        return this;
    }

    /// <summary>
    /// Sets the page size for the pagination criteria.
    /// </summary>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A reference to itself for method chaining.</returns>
    public PaginationCriteriaConfigurator<T> UsePageSize(int pageSize)
    {
        _paginationCriteria.PageSize = pageSize;

        return this;
    }

    /// <summary>
    /// Adds a sorting criterion to the pagination criteria based on a specified property name and direction.
    /// </summary>
    /// <param name="propertyName">The name of the property to sort by.</param>
    /// <param name="direction">The sort direction.</param>
    /// <returns>A reference to itself for method chaining.</returns>
    public PaginationCriteriaConfigurator<T> SortBy(string propertyName,
        SortDirection direction = SortDirection.Ascending)
    {
        _paginationCriteria.SortCriteria.Add(new SortCriterion<T>(propertyName, direction));

        return this;
    }

    /// <summary>
    /// Adds a sorting criterion based on a specified property and direction to the pagination criteria.
    /// </summary>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    /// <param name="propertySelector">A lambda expression representing the property to sort by.</param>
    /// <param name="direction">The sort direction.</param>
    /// <returns>A reference to itself for method chaining.</returns>
    public PaginationCriteriaConfigurator<T> SortBy<TProperty>(
        Expression<Func<T, TProperty>> propertySelector,
        SortDirection direction = SortDirection.Ascending)
    {
        _paginationCriteria.SortCriteria.Add(new SortCriterion<T, TProperty>(propertySelector, direction));

        return this;
    }
}
