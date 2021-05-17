// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// A sorting directive consisting of a sort property and sort direction.
    /// </summary>
    /// <typeparam name="T">The type of the object whose property to sort by.</typeparam>
    public class SortProperty<T> : ISortDirective<T>
    {
        #region Constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="SortProperty{T}"/> class given the name of the property to sort by and the sort direction.
        /// </summary>
        /// <param name="propertyName">The name of the property on the target object to sort by. Supports, to some extent, the specification of nested paths using dot notation.</param>
        /// <param name="direction">The direction in which to sort.</param>
        public SortProperty(string propertyName, SortDirection direction) : this(direction)
        {
            Ensure.Argument.NotNullOrWhiteSpace(propertyName, nameof(propertyName));
            Property = GetPropertySelector(propertyName);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortProperty{T}"/> class given the sort direction.
        /// </summary>
        /// <param name="direction">The direction in which to sort.</param>
        protected SortProperty(SortDirection direction) => Direction = direction;
        #endregion

        /// <summary>
        /// A lambda expression selecting the property on the target object to sort by.
        /// </summary>
        public LambdaExpression Property { get; }

        /// <summary>
        /// The direction in which to sort.
        /// </summary>
        public SortDirection Direction { get; }

        /// <inheritdoc/>
        public virtual IOrderedQueryable<T> ApplyTo(IQueryable<T> sequence)
        {
            var sortMethodName = sequence.IsOrdered()
                ? Direction == SortDirection.Ascending
                    ? "ThenBy"
                    : "ThenByDescending"
                : Direction == SortDirection.Ascending
                    ? "OrderBy"
                    : "OrderByDescending";
            
            var sortMethod = typeof(Queryable).GetMethods().Single(method => method.Name == sortMethodName && method.GetParameters().Length == 2);
            sortMethod = sortMethod.MakeGenericMethod(typeof(T), Property.ReturnType);

            return (IOrderedQueryable<T>)sortMethod.Invoke(null, new object[] { sequence, Property });
        }

        private static LambdaExpression GetPropertySelector(string propertyName)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            Expression expression = parameter;
            foreach (var nestedProperty in propertyName.Split('.'))
            {
                var property = type.GetPublicProperty(nestedProperty);
                if (property is null)
                {
                    throw new ArgumentException($"No property '{nestedProperty}' on type '{type.Name}'.", nameof(propertyName));
                }

                expression = Expression.Property(expression, property);
                type = property.PropertyType;
            }

            expression = Expression.Lambda(expression, parameter);
            return expression as LambdaExpression;
        }
    }
}
