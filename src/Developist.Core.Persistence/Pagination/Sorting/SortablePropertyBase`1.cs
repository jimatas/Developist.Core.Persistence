using System.Linq;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Represents a base class for sortable properties.
    /// </summary>
    /// <typeparam name="T">The type of the items to sort.</typeparam>
    public abstract class SortablePropertyBase<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SortablePropertyBase{T}"/> class with the specified sort direction.
        /// </summary>
        /// <param name="direction">The sort direction.</param>
        protected SortablePropertyBase(SortDirection direction) => Direction = direction;

        /// <summary>
        /// Gets the sort direction.
        /// </summary>
        public SortDirection Direction { get; }

        /// <summary>
        /// Sorts the specified query by this property.
        /// </summary>
        /// <param name="query">The query to sort.</param>
        /// <returns>The sorted query.</returns>
        public abstract IOrderedQueryable<T> Sort(IQueryable<T> query);
    }
}
