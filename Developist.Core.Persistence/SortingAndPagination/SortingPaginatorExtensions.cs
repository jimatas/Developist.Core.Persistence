// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public static class SortingPaginatorExtensions
    {
        /// <summary>
        /// Sets the <see cref="SortingPaginator{T}.PageNumber "/> property of <paramref name="paginator"/> to the specified value. 
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to set the PageNumber property of.</param>
        /// <param name="pageNumber">The desired page number.</param>
        /// <returns>A reference to the paginator instance to allow for method chaining.</returns>
        public static SortingPaginator<T> StartingAt<T>(this SortingPaginator<T> paginator, int pageNumber)
        {
            paginator.PageNumber = pageNumber;
            return paginator;
        }

        /// <summary>
        /// Sets the <see cref="SortingPaginator{T}.PageSize"/> property of <paramref name="paginator"/> to the specified value.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to set the PageSize property of.</param>
        /// <param name="pageSize">The desired page size.</param>
        /// <returns>A reference to the paginator instance to allow for method chaining.</returns>
        public static SortingPaginator<T> WithPageSizeOf<T>(this SortingPaginator<T> paginator, int pageSize)
        {
            paginator.PageSize = pageSize;
            return paginator;
        }

        /// <summary>
        /// Adds a new sorting directive to the <see cref="SortingPaginator{T}.SortProperties"/> collection of the <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <param name="paginator">The paginator to add the sort directive to.</param>
        /// <param name="property">The name of the property to sort by.</param>
        /// <param name="direction">The direction in which to sort. Defaults to <see cref="SortDirection.Ascending"/>.</param>
        /// <returns>A reference to the paginator to allow for chaining of method calls.</returns>
        public static SortingPaginator<T> SortedBy<T>(this SortingPaginator<T> paginator, string property, SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortProperties.Add(new SortProperty<T>(property, direction));
            return paginator;
        }

        /// <summary>
        /// Adds a new sorting directive to the <see cref="SortingPaginator{T}.SortProperties"/> collection of the <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items being paginated over.</typeparam>
        /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
        /// <param name="paginator">The paginator to add the sort directive to.</param>
        /// <param name="property">A lambda expression selecting the property to sort by on the target object.</param>
        /// <param name="direction">The direction in which to sort. Defaults to <see cref="SortDirection.Ascending"/>.</param>
        /// <returns>A reference to the paginator to allow for chaining of method calls.</returns>
        public static SortingPaginator<T> SortedBy<T, TProperty>(this SortingPaginator<T> paginator, Expression<Func<T, TProperty>> property, SortDirection direction = SortDirection.Ascending)
        {
            paginator.SortProperties.Add(new SortProperty<T, TProperty>(property, direction));
            return paginator;
        }

        /// <summary>
        /// Determines whether there is another page within the result set.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is only considered accurate after a call to <see cref="SortingPaginator{T}.Paginate"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items being paginated over.</typeparam>
        /// <param name="paginator">The result set paginator.</param>
        /// <returns><see langword="true"/> if there is another page of items left in the result set, <see langword="false"/> otherwise.</returns>
        public static bool HasNextPage<T>(this SortingPaginator<T> paginator) => paginator.PageNumber < paginator.PageCount;

        /// <summary>
        /// Determines whether there is a previous page within the result set.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is only considered accurate after a call to <see cref="SortingPaginator{T}.Paginate"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items being paginated over.</typeparam>
        /// <param name="paginator">The result set paginator.</param>
        /// <returns><see langword="true"/> if there is a preceding page of items in the result set, <see langword="false"/> otherwise.</returns>
        public static bool HasPreviousPage<T>(this SortingPaginator<T> paginator) => paginator.PageNumber > 1;

        /// <summary>
        /// Checks if there is another page within the result set, and if so, increments the <see cref="SortingPaginator{T}.PageNumber"/> property of the <paramref name="paginator"/>.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is only considered accurate after a call to <see cref="SortingPaginator{T}.Paginate"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items being paginated over.</typeparam>
        /// <param name="paginator">The result set paginator.</param>
        /// <returns><see langword="true"/> if there was another page of items left in the result set, <see langword="false"/> otherwise.</returns>
        public static bool MoveNextPage<T>(this SortingPaginator<T> paginator)
        {
            if (paginator.HasNextPage())
            {
                paginator.PageNumber++;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Checks if there is a previous page within the result set, and if so, decrements the <see cref="SortingPaginator{T}.PageNumber"/> property of the <paramref name="paginator"/>.
        /// </summary>
        /// <remarks>
        /// The value returned by this method is only considered accurate after a call to <see cref="SortingPaginator{T}.Paginate"/>.
        /// </remarks>
        /// <typeparam name="T">The type of items being paginated over.</typeparam>
        /// <param name="paginator">The result set paginator.</param>
        /// <returns><see langword="true"/> if there was a preceding page of items in the result set, <see langword="false"/> otherwise.</returns>
        public static bool MovePreviousPage<T>(this SortingPaginator<T> paginator)
        {
            if (paginator.HasPreviousPage())
            {
                paginator.PageNumber--;
                return true;
            }
            return false;
        }
    }
}
