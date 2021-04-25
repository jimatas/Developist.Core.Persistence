﻿// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

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
        public const int DefaultPageSize = 20;

        private int pageNumber;
        private int pageSize;
        private int itemCount;

        #region Constructors
        public SortingPaginator() : this(pageNumber: 1) { }
        public SortingPaginator(int pageNumber) : this(pageNumber, DefaultPageSize) { }
        public SortingPaginator(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        #endregion

        /// <summary>
        /// The 1-based page number.
        /// </summary>
        public int PageNumber
        {
            get => pageNumber;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageNumber), "Value cannot be less than one.");
                }
                pageNumber = value;
            }
        }

        /// <summary>
        /// The number of items per page.
        /// </summary>
        public int PageSize
        {
            get => pageSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageSize), "Value cannot be less than one.");
                }
                pageSize = value;
            }
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
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ItemCount), "Value cannot be negative.");
                }
                itemCount = value;

                PageCount = (int)Math.Ceiling((double)itemCount / PageSize);
            }
        }

        /// <summary>
        /// The properties to sort by including their sort directions.
        /// </summary>
        public ICollection<SortProperty<T>> SortProperties { get; } = new HashSet<SortProperty<T>>();

        public IQueryable<T> Paginate(IQueryable<T> sequence)
        {
            ItemCount = sequence.Count();

            IOrderedQueryable<T> orderedSequence = null;

            foreach (var property in SortProperties)
            {
                if (property.Direction == SortDirection.Ascending)
                {
                    orderedSequence = orderedSequence is null
                        ? sequence.OrderBy(property.Expression)
                        : orderedSequence.ThenBy(property.Expression);

                }
                else if (property.Direction == SortDirection.Descending)
                {
                    orderedSequence = orderedSequence is null
                        ? sequence.OrderByDescending(property.Expression)
                        : orderedSequence.ThenByDescending(property.Expression);
                }
            }

            if (orderedSequence is not null)
            {
                sequence = orderedSequence.Skip((PageNumber - 1) * PageSize);
            }

            sequence = sequence.Take(PageSize);

            return sequence;
        }
    }
}