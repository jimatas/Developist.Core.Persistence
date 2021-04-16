// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Default implementation of the <see cref="IQueryablePaginator{T}"/> interface that can both sort and paginate the elements in a sequence.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
    public class SorterPaginator<T> : IQueryablePaginator<T>
    {
        public const int DefaultPageSize = 20;

        private string sortProperty;
        private int pageNumber = 1;
        private int pageSize = DefaultPageSize;
        private int itemCount;

        #region Constructors
        public SorterPaginator() { }
        public SorterPaginator(string propertyName, bool descending) : this(propertyName, descending, pageNumber: 1, DefaultPageSize) { }
        public SorterPaginator(int pageNumber, int pageSize) : this(propertyName: null, descending: false, pageNumber, pageSize) { }
        public SorterPaginator(string propertyName, int pageNumber, int pageSize) : this(propertyName, descending: false, pageNumber, pageSize) { }
        public SorterPaginator(string propertyName, bool descending, int pageNumber, int pageSize)
        {
            SortProperty = propertyName;
            SortDescending = descending;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public SorterPaginator(SorterPaginator<T> paginator)
            : this(paginator.SortProperty,
                   paginator.SortDescending,
                   paginator.PageNumber,
                   paginator.PageSize)
        {
            ItemCount = paginator.ItemCount;
            SortExpression = paginator.SortExpression;
        }
        #endregion

        /// <summary>
        /// The page number. The first page is 1, the second 2 etc.
        /// </summary>
        public int PageNumber
        {
            get => pageNumber;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageNumber));
                }
                pageNumber = value;
            }
        }

        /// <summary>
        /// The number of elements per page.
        /// </summary>
        public int PageSize
        {
            get => pageSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(PageSize));
                }
                pageSize = value;
            }
        }

        /// <summary>
        /// The number of pages needed to paginate all the elements in the result set.
        /// </summary>
        public int PageCount { get; private set; }

        /// <summary>
        /// The total number of elements in the result set.
        /// </summary>
        public int ItemCount
        {
            get => itemCount;
            private set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(ItemCount));
                }
                itemCount = value;

                PageCount = (int)Math.Ceiling((double)itemCount / PageSize);
            }
        }

        /// <summary>
        /// The name of the property to sort by.
        /// </summary>
        /// <remarks>
        /// Provides some basic support for specifying nested properties using dot notation.
        /// </remarks>
        public string SortProperty
        {
            get => sortProperty;
            set
            {
                if (!string.Equals(sortProperty, value))
                {
                    sortProperty = value;
                    SortExpression = string.IsNullOrEmpty(sortProperty) ? null : GetPropertySelector(sortProperty);
                }
            }
        }

        /// <summary>
        /// A lambda expression that selects the property to sort by.
        /// </summary>
        public Expression<Func<T, object>> SortExpression { get; internal set; }

        /// <summary>
        /// Indicates whether sorting will be in descending order instead of the (default) ascending.
        /// </summary>
        public bool SortDescending { get; set; }

        public IOrderedQueryable<T> Paginate(IQueryable<T> sequence)
        {
            ItemCount = sequence.Count();

            if (SortExpression is not null)
            {
                if (SortDescending)
                {
                    sequence = sequence.OrderByDescending(SortExpression);
                }
                else
                {
                    sequence = sequence.OrderBy(SortExpression);
                }

                sequence = sequence.Skip((PageNumber - 1) * PageSize);
            }

            sequence = sequence.Take(PageSize);

            return (IOrderedQueryable<T>)sequence;
        }

        private static Expression<Func<T, object>> GetPropertySelector(string propertyName)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            Expression expression = parameter;

            foreach (var nestedPropertyName in propertyName.Split('.'))
            {
                var property = type.GetPublicProperty(nestedPropertyName);
                if (property is null)
                {
                    throw new ArgumentException($"No property '{nestedPropertyName}' on type '{type.Name}'.", nameof(propertyName));
                }

                expression = Expression.Property(expression, property);
                type = property.PropertyType;
            }

            expression = Expression.Convert(expression, typeof(object));
            return Expression.Lambda<Func<T, object>>(expression, parameter);
        }
    }
}
