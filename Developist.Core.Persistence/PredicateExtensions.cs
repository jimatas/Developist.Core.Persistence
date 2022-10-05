using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    public static class PredicateExtensions
    {
        public static Func<T, bool> AndAlso<T>(this Func<T, bool> predicate, Func<T, bool> otherPredicate)
            => arg => predicate(arg) && otherPredicate(arg);

        public static Expression<Func<T, bool>> AndAlso<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> otherPredicate)
            => Expression.Lambda<Func<T, bool>>(
                body: Expression.AndAlso(
                    left: predicate.Body,
                    right: otherPredicate.ReplaceParameterWith(predicate.Parameters.Single()).Body),
                parameters: predicate.Parameters);

        public static Func<T, bool> OrElse<T>(this Func<T, bool> predicate, Func<T, bool> otherPredicate)
            => arg => predicate(arg) || otherPredicate(arg);

        public static Expression<Func<T, bool>> OrElse<T>(this Expression<Func<T, bool>> predicate, Expression<Func<T, bool>> otherPredicate)
            => Expression.Lambda<Func<T, bool>>(
                body: Expression.OrElse(
                    left: predicate.Body,
                    right: otherPredicate.ReplaceParameterWith(predicate.Parameters.Single()).Body),
                parameters: predicate.Parameters);

        private static TExpression ReplaceParameterWith<TExpression>(this TExpression expression, ParameterExpression replacement)
            where TExpression : Expression
            => new ParameterReplacer(replacement).Visit(expression);

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression replacement;
            public ParameterReplacer(ParameterExpression replacement)
                => this.replacement = replacement;

            public TExpression Visit<TExpression>(TExpression expression)
                where TExpression : Expression
                => (TExpression)base.Visit(expression);

            protected override Expression VisitParameter(ParameterExpression parameter)
                => base.VisitParameter(replacement);
        }
    }
}
