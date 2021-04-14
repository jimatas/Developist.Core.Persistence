// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public static class PredicateExtensions
    {
        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate, Func<T, bool> otherPredicate) => arg => predicate(arg) && otherPredicate(arg);

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> otherPredicate)
            => Expression.Lambda<Func<T, bool>>(Expression.AndAlso(predicate.Body, ParameterReplacer.Replace(predicate.Parameters.Single(), otherPredicate).Body), predicate.Parameters);

        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate, Func<T, bool> otherPredicate) => arg => predicate(arg) || otherPredicate(arg);

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> otherPredicate)
            => Expression.Lambda<Func<T, bool>>(Expression.OrElse(predicate.Body, ParameterReplacer.Replace(predicate.Parameters.Single(), otherPredicate).Body), predicate.Parameters);

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression replacement;
            public ParameterReplacer(ParameterExpression replacement) => this.replacement = replacement;
            protected override Expression VisitParameter(ParameterExpression parameter) => base.VisitParameter(replacement);
            public static TExpression Replace<TExpression>(ParameterExpression replacement, TExpression expression)
                where TExpression : Expression => (TExpression)new ParameterReplacer(replacement).Visit(expression);
        }
    }
}
