// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public class SortProperty<T> : ISortProperty<T>
    {
        #region Constructors
        public SortProperty(string property, SortDirection direction) : this(direction)
        {
            Property = Ensure.Argument.NotNullOrWhiteSpace(property, nameof(property));
        }

        protected SortProperty(SortDirection direction) => Direction = direction;
        #endregion

        /// <summary>
        /// The name of the property to sort by.
        /// </summary>
        /// <remarks>
        /// Supports, to some extent, specifying nested paths using dot notation.
        /// </remarks>
        public string Property { get; }

        /// <summary>
        /// The direction in which to sort.
        /// </summary>
        public SortDirection Direction { get; }

        public virtual IOrderedQueryable<T> ApplyTo(IQueryable<T> sequence)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            Expression expression = parameter;
            foreach (var nestedProperty in Property.Split('.'))
            {
                var property = type.GetProperty(nestedProperty);
                if (property is null)
                {
                    throw new InvalidOperationException($"No property '{nestedProperty}' on type '{type.Name}'.");
                }

                expression = Expression.Property(expression, nestedProperty);
                type = property.PropertyType;
            }
            expression = Expression.Lambda(expression, parameter);

            var sortMethodName = sequence.Expression.Type == typeof(IOrderedQueryable<T>)
                ? Direction == SortDirection.Ascending
                    ? "ThenBy"
                    : "ThenByDescending"
                : Direction == SortDirection.Ascending
                    ? "OrderBy"
                    : "OrderByDescending";

            var sortMethod = typeof(Queryable).GetMethods().Single(method => method.Name == sortMethodName && method.GetParameters().Length == 2);
            sortMethod = sortMethod.MakeGenericMethod(typeof(T), type);

            return (IOrderedQueryable<T>)sortMethod.Invoke(null, new object[] { sequence, expression });
        }
    }
}
