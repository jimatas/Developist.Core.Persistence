// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Default implementation of the <see cref="IQueryablePaginator{T}"/> interface that both sorts and paginates a result set.
    /// </summary>
    /// <typeparam name="T">The type of the items in result set.</typeparam>
    public class SortingPaginator<T> : IQueryablePaginator<T>
    {
        /// <summary>
        /// The default page size to use when one is not specified.
        /// </summary>
        public const int DefaultPageSize = 20;

        private int pageNumber;
        private int pageSize;
        private int itemCount;

        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class.
        /// </summary>
        public SortingPaginator() : this(pageNumber: 1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class given the current page number.
        /// </summary>
        /// <param name="pageNumber">The 1-based page number.</param>
        public SortingPaginator(int pageNumber) : this(pageNumber, DefaultPageSize) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class given the current page number and page size.
        /// </summary>
        /// <param name="pageNumber">The 1-based page number.</param>
        /// <param name="pageSize">The number of items per page.</param>
        public SortingPaginator(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        #endregion

        /// <summary>
        /// The page number. The first page is 1, the second is 2, etc.
        /// </summary>
        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = Ensure.Argument.NotOutOfRange(value, nameof(PageNumber), message: "Value cannot be negative or zero.", lowerBound: 1);
        }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize
        {
            get => pageSize;
            set => pageSize = Ensure.Argument.NotOutOfRange(value, nameof(PageSize), message: "Value cannot be negative or zero.", lowerBound: 1);
        }

        /// <summary>
        /// The number of pages needed to paginate all the items in the result set.
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// The total number of items in the result set.
        /// </summary>
        public int ItemCount
        {
            get => itemCount;
            private set
            {
                itemCount = Ensure.Argument.NotOutOfRange(value, nameof(ItemCount), message: "Value cannot be negative.", lowerBound: 0);
                PageCount = (int)Math.Ceiling((double)itemCount / PageSize);
            }
        }

        /// <summary>
        /// The instructions by which to sort the items in the result set.
        /// </summary>
        public ICollection<ISortDirective<T>> SortDirectives { get; } = new List<ISortDirective<T>>();

        /// <inheritdoc/>
        public IQueryable<T> Paginate(IQueryable<T> sequence)
        {
            ItemCount = sequence.Count();

            IOrderedQueryable<T> orderedSequence = null;
            foreach (var directive in SortDirectives)
            {
                orderedSequence = directive.ApplyTo(orderedSequence ?? sequence);
            }

            if (orderedSequence is not null)
            {
                sequence = orderedSequence.Skip((PageNumber - 1) * PageSize);
            }
            sequence = sequence.Take(PageSize);

            return sequence;
        }

        // Note: The semantics are those of the pre-increment operator.
        public static SortingPaginator<T> operator ++(SortingPaginator<T> paginator)
        {
            paginator.PageNumber++;
            return paginator;
        }

        // Note: The semantics are those of the pre-decrement operator.
        public static SortingPaginator<T> operator --(SortingPaginator<T> paginator)
        {
            paginator.PageNumber--; // Will throw ArgumentOutOfRangeException if PageNumber < 1.
            return paginator;
        }
    }
}
