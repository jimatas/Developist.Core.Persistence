// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public static class SortingPaginatorExtensions
    {
        /// <summary>
        /// Sorts by the property with the specified name.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to add the sort property to.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <returns>A reference to the paginator to which the sort property has been added.</returns>
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string propertyName)
            => paginator.SortedBy(propertyName, SortDirection.Ascending);

        /// <summary>
        /// Sorts by the property with the specified name. This overload also allows for specifying the sort direction.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to add the sort property to.</param>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <param name="direction">The direction in which to sort.</param>
        /// <returns>A reference to the paginator to which the sort property has been added.</returns>
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string propertyName, SortDirection direction)
        {
            paginator.SortProperties.Add(new(propertyName, direction));
            return paginator;
        }

        /// <summary>
        /// Sorts by the property specified by the lambda expression.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to add the sort property to.</param>
        /// <param name="propertySelector">A lambda expression that selects the property to sort by on the target object.</param>
        /// <returns>A reference to the paginator to which the sort property has been added.</returns>
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, Expression<Func<T, object>> propertySelector)
            => paginator.SortedBy(propertySelector, SortDirection.Ascending);

        /// <summary>
        /// Sorts by the property specified by the lambda expression. This overload also allows for specifying the sort direction.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to add the sort property to.</param>
        /// <param name="propertySelector">A lambda expression that selects the property to sort by on the target object.</param>
        /// <param name="direction">The direction in which to sort.</param>
        /// <returns>A reference to the paginator to which the sort property has been added.</returns>
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, Expression<Func<T, object>> propertySelector, SortDirection direction)
        {
            paginator.SortProperties.Add(new(propertySelector, direction));
            return paginator;
        }
    }
}
