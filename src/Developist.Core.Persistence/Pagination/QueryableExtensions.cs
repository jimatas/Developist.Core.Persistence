using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Provides extension methods for working with <see cref="IQueryable{T}"/> objects.
    /// </summary>
    public static partial class QueryableExtensions
    {
        /// <summary>
        /// Converts the specified query to a paginated list using the specified <paramref name="paginator"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the query.</typeparam>
        /// <param name="query">The query to paginate.</param>
        /// <param name="paginator">The paginator to use for pagination.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task that represents the asynchronous operation. The result of the task is the paginated list of items.</returns>
        public static Task<IPaginatedList<T>> ToPaginatedListAsync<T>(this IQueryable<T> query, IPaginator<T> paginator, CancellationToken cancellationToken = default)
        {
            if (paginator is null)
            {
                throw new ArgumentNullException(nameof(paginator));
            }

            return paginator.PaginateAsync(query, cancellationToken);
        }

        /// <summary>
        /// Sorts the specified query by the specified <paramref name="property"/>.
        /// </summary>
        /// <typeparam name="T">The type of the items in the query.</typeparam>
        /// <param name="query">The query to sort.</param>
        /// <param name="property">The property to sort by.</param>
        /// <returns>The sorted query.</returns>
        public static IOrderedQueryable<T> SortBy<T>(this IQueryable<T> query, SortablePropertyBase<T> property)
        {
            if (property is null)
            {
                throw new ArgumentNullException(nameof(property));
            }

            return property.Sort(query);
        }

        internal static bool IsSorted<T>(this IQueryable<T> query)
        {
            var detector = new SortMethodDetector();
            detector.Visit(query.Expression);

            return detector.IsSortMethodDetected;
        }

        private class SortMethodDetector : ExpressionVisitor
        {
            private static readonly string[] SortMethodNames = new[]
            {
                "OrderBy",
                "OrderByDescending",
                "ThenBy",
                "ThenByDescending"
            };

            public bool IsSortMethodDetected { get; private set; }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (SortMethodNames.Contains(node.Method.Name))
                {
                    IsSortMethodDetected = true;
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}
