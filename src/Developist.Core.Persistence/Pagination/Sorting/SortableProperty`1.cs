using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination.Sorting
{
    /// <summary>
    /// Represents a sortable property with a property selector for sorting <typeparamref name="T"/> items.
    /// </summary>
    /// <typeparam name="T">The type of the items to sort.</typeparam>
    public class SortableProperty<T> : SortablePropertyBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortableProperty{T}"/> class with the specified property name and sort direction.
        /// </summary>
        /// <param name="propertyName">The name of the property to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        public SortableProperty(string propertyName, SortDirection direction)
            : base(direction)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(
                    message: "Value cannot be null, empty, or contain only whitespace characters.",
                    paramName: nameof(propertyName));
            }

            Property = GetPropertySelector(propertyName);
        }

        /// <summary>
        /// Gets the property selector.
        /// </summary>
        public LambdaExpression Property { get; }

        /// <inheritdoc/>
        public override IOrderedQueryable<T> Sort(IQueryable<T> query)
        {
            var sortMethodName = query.IsSorted()
                ? Direction == SortDirection.Ascending
                    ? "ThenBy"
                    : "ThenByDescending"
                : Direction == SortDirection.Ascending
                    ? "OrderBy"
                    : "OrderByDescending";

            var sortMethod = typeof(Queryable).GetMethods().Single(method => method.Name.Equals(sortMethodName) && method.GetParameters().Length == 2);
            sortMethod = sortMethod.MakeGenericMethod(typeof(T), Property.ReturnType);

            return (IOrderedQueryable<T>)sortMethod.Invoke(null, new object[] { query, Property });
        }

        private static LambdaExpression GetPropertySelector(string propertyName)
        {
            var type = typeof(T);
            var parameter = Expression.Parameter(type, "p");

            Expression expression = parameter;
            foreach (var nestedProperty in propertyName.Split('.'))
            {
                var property = type.GetProperty(nestedProperty)
                    ?? throw new ArgumentException(
                        message: $"No accessible property '{nestedProperty}' defined on type '{type.Name}'.",
                        paramName: nameof(propertyName));

                expression = Expression.Property(expression, property);
                type = property.PropertyType;
            }

            expression = Expression.Lambda(expression, parameter);

            return (LambdaExpression)expression;
        }
    }
}
