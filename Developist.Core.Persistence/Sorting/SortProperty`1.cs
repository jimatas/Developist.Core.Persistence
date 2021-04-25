// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq.Expressions;
using LinqExpression = System.Linq.Expressions.Expression;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Specifies the property to sort by and in which direction.
    /// </summary>
    /// <typeparam name="T">The type of the object whose property to sort by.</typeparam>
    public class SortProperty<T>
    {
        #region Constructors
        public SortProperty(string name, SortDirection direction)
            : this(GetPropertySelector(name), direction) => Name = name;

        public SortProperty(Expression<Func<T, object>> expression, SortDirection direction)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Direction = direction;
        }
        #endregion

        /// <summary>
        /// The name of the property to sort by. 
        /// </summary>
        /// <value>Only set when a property name rather than an expression was specified in the constructor call.</value>
        public string Name { get; }

        /// <summary>
        /// A lambda expression that selects the property to sort by on the target object.
        /// </summary>
        public Expression<Func<T, object>> Expression { get; }

        /// <summary>
        /// The direction in which to sort.
        /// </summary>
        public SortDirection Direction { get; }

        private static Expression<Func<T, object>> GetPropertySelector(string name)
        {
            if (name is null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (name.Trim().Length == 0)
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", nameof(name));
            }

            var type = typeof(T);
            var parameter = LinqExpression.Parameter(type, "p");

            LinqExpression expression = parameter;

            foreach (var nestedPropertyName in name.Split('.'))
            {
                var property = type.GetPublicProperty(nestedPropertyName);
                if (property is null)
                {
                    throw new ArgumentException($"No property '{nestedPropertyName}' on type '{type.Name}'.", nameof(name));
                }

                expression = LinqExpression.Property(expression, property);
                type = property.PropertyType;
            }

            expression = LinqExpression.Convert(expression, typeof(object));
            return LinqExpression.Lambda<Func<T, object>>(expression, parameter);
        }
    }
}
