// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    internal static class QueryableExtensions
    {
        /// <summary>
        /// Determines whether the sequence is sorted.
        /// </summary>
        /// <remarks>
        /// A sequence is considered sorted if OrderBy(Descending) or ThenBy(Descending) has been called on it.
        /// </remarks>
        /// <typeparam name="T">The type of the elements in the sequence.</typeparam>
        /// <param name="sequence">The sequence to determine </param>
        /// <returns><see langword="true"/> if <paramref name="sequence"/> is sorted, otherwise <see langword="false"/>.</returns>
        public static bool IsOrdered<T>(this IQueryable<T> sequence) => OrderMethodFinder.Find(sequence.Expression);

        /// <summary>
        /// Simply doing a check such as <c>queryable.Expression.Type == typeof(IOrderedQueryable&lt;T&gt;)</c> won't work 
        /// if OrderBy(Descending) or ThenBy(Descending) wasn't the last operation performed on the queryable.
        /// </summary>
        private class OrderMethodFinder : ExpressionVisitor
        {
            public bool Found { get; private set; }

            public static bool Find(Expression expression)
            {
                var finder = new OrderMethodFinder();
                finder.Visit(expression);

                return finder.Found;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (new[] { "OrderBy", "OrderByDescending", "ThenBy", "ThenByDescending" }.Contains(node.Method.Name))
                {
                    Found = true;
                }

                return base.VisitMethodCall(node);
            }
        }
    }
}
