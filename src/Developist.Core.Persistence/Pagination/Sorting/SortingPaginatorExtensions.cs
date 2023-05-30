using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination.Sorting
{
    /// <summary>
    /// Provides extension methods for configuring instances of <see cref="SortingPaginator{T}"/>.
    /// </summary>
    public static class SortingPaginatorExtensions
    {
        /// <summary>
        /// Sets the starting page number for the specified <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items to paginate.</typeparam>
        /// <param name="paginator">The paginator to configure.</param>
        /// <param name="pageNumber">The starting page number.</param>
        /// <returns>The same instance of the <paramref name="paginator"/>.</returns>
        public static SortingPaginator<T> StartingAtPage<T>(this SortingPaginator<T> paginator, int pageNumber)
        {
            paginator.PageNumber = pageNumber;

            return paginator;
        }

        /// <summary>
        /// Sets the page size for the specified <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items to paginate.</typeparam>
        /// <param name="paginator">The paginator to configure.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>The same instance of the <paramref name="paginator"/>.</returns>
        public static SortingPaginator<T> WithPageSize<T>(this SortingPaginator<T> paginator, int pageSize)
        {
            paginator.PageSize = pageSize;

            return paginator;
        }

        /// <summary>
        /// Adds a sortable property with the specified property name and sort direction to the specified <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items to paginate.</typeparam>
        /// <param name="paginator">The paginator to configure.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>The same instance of the <paramref name="paginator"/>.</returns>
        public static SortingPaginator<T> SortedByProperty<T>(
            this SortingPaginator<T> paginator,
            string propertyName,
            SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortableProperties.Add(new SortableProperty<T>(propertyName, direction));

            return paginator;
        }

        /// <summary>
        /// Adds a sortable property with the specified property expression and sort direction to the specified <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items to paginate.</typeparam>
        /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
        /// <param name="paginator">The paginator to configure.</param>
        /// <param name="property">The property expression to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        /// <returns>The same instance of the <paramref name="paginator"/>.</returns>
        public static SortingPaginator<T> SortedByProperty<T, TProperty>(
            this SortingPaginator<T> paginator,
            Expression<Func<T, TProperty>> property,
            SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortableProperties.Add(new SortableProperty<T, TProperty>(property, direction));

            return paginator;
        }

        /// <summary>
        /// Adds sortable properties parsed from the specified sort string to the specified <paramref name="paginator"/>.
        /// </summary>
        /// <remarks>
        /// The <paramref name="sortString"/> parameter contains one or more sorting directives, separated by commas. 
        /// Each sorting directive specifies the name of a property to sort by, and optionally, the sort direction.
        /// The direction is specified by prepending either a plus sign '+' or a minus sign '-' to the property name. 
        /// If no sign is specified, the default direction is ascending. If the minus sign is used, the direction is descending.
        /// </remarks>
        /// <typeparam name="T">The type of the items to paginate.</typeparam>
        /// <param name="paginator">The paginator to configure.</param>
        /// <param name="sortString">The sort string to parse.</param>
        public static SortingPaginator<T> SortedByString<T>(this SortingPaginator<T> paginator, string sortString)
        {
            if (string.IsNullOrWhiteSpace(sortString))
            {
                throw new ArgumentException(
                    message: "Value cannot be null, empty, or contain only whitespace characters.",
                    paramName: nameof(sortString));
            }

            foreach (var property in ParseSortingDirectives())
            {
                paginator.SortableProperties.Add(property);
            }

            return paginator;

            IEnumerable<SortableProperty<T>> ParseSortingDirectives()
            {
                foreach (var directive in sortString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                {
                    var propertyName = directive;

                    bool descending;
                    if ((descending = directive.StartsWith("-")) || directive.StartsWith("+"))
                    {
                        propertyName = directive.Substring(1).TrimStart('(').TrimEnd(')');
                    }
                    var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                    SortableProperty<T> property;
                    try
                    {
                        property = new SortableProperty<T>(propertyName, direction);
                    }
                    catch (ArgumentException exception)
                    {
                        throw new FormatException("Failed to parse a sorting directive from the input string. See the inner exception for details.", exception);
                    }

                    yield return property;
                }
            }
        }
    }
}
