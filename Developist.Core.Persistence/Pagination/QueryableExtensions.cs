using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    internal static class QueryableExtensions
    {
        public static bool IsSorted<T>(this IQueryable<T> query) => SortMethodFinder.IsSorted(query.Expression);

        private class SortMethodFinder : ExpressionVisitor
        {
            public bool IsSortMethodFound { get; private set; }

            public static bool IsSorted(Expression expression)
            {
                var finder = new SortMethodFinder();
                finder.Visit(expression);
                return finder.IsSortMethodFound;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (new[] { "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" }.Contains(node.Method.Name))
                {
                    IsSortMethodFound = true;
                }
                return base.VisitMethodCall(node);
            }
        }
    }
}
