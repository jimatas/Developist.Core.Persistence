using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination.Sorting
{
    /// <summary>
    /// Represents a sortable property with an expression for sorting <typeparamref name="T"/> items by the specified <typeparamref name="TProperty"/>.
    /// </summary>
    /// <typeparam name="T">The type of the items to sort.</typeparam>
    /// <typeparam name="TProperty">The type of the property to sort by.</typeparam>
    public class SortableProperty<T, TProperty> : SortablePropertyBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortableProperty{T, TProperty}"/> class with the specified property expression and sort direction.
        /// </summary>
        /// <param name="property">The property expression to sort by.</param>
        /// <param name="direction">The sort direction.</param>
        public SortableProperty(Expression<Func<T, TProperty>> property, SortDirection direction)
            : base(direction)
        {
            Property = property ?? throw new ArgumentNullException(nameof(property));
        }

        /// <summary>
        /// Gets the property expression.
        /// </summary>
        public Expression<Func<T, TProperty>> Property { get; }

        /// <inheritdoc/>
        public override IOrderedQueryable<T> Sort(IQueryable<T> query)
        {
            return query.IsSorted()
                ? Direction == SortDirection.Ascending
                    ? ((IOrderedQueryable<T>)query).ThenBy(Property)
                    : ((IOrderedQueryable<T>)query).ThenByDescending(Property)
                : Direction == SortDirection.Ascending
                    ? query.OrderBy(Property)
                    : query.OrderByDescending(Property);
        }
    }
}
