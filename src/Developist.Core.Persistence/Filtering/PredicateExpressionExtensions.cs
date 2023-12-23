using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.Filtering;

/// <summary>
/// Provides extension methods for combining predicate expressions.
/// </summary>
public static class PredicateExpressionExtensions
{
    /// <summary>
    /// Combines two predicate expressions using a logical AND operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expression.</typeparam>
    /// <param name="predicate">The first predicate expression.</param>
    /// <param name="otherPredicate">The second predicate expression to combine.</param>
    /// <returns>A new expression that represents the logical AND of the two input predicates.</returns>
    public static Expression<Func<T, bool>> AndAlso<T>(
        this Expression<Func<T, bool>> predicate,
        Expression<Func<T, bool>> otherPredicate)
    {
        Ensure.NotNull(otherPredicate);

        return Expression.Lambda<Func<T, bool>>(
            body: Expression.AndAlso(
                left: predicate.Body,
                right: ParameterReplacer.Replace(predicate.Parameters.Single(), otherPredicate).Body),
            predicate.Parameters);
    }

    /// <summary>
    /// Combines two predicate expressions using a logical OR operation.
    /// </summary>
    /// <typeparam name="T">The type of the parameter in the expression.</typeparam>
    /// <param name="predicate">The first predicate expression.</param>
    /// <param name="otherPredicate">The second predicate expression to combine.</param>
    /// <returns>A new expression that represents the logical OR of the two input predicates.</returns>
    public static Expression<Func<T, bool>> OrElse<T>(
        this Expression<Func<T, bool>> predicate,
        Expression<Func<T, bool>> otherPredicate)
    {
        Ensure.NotNull(otherPredicate);

        return Expression.Lambda<Func<T, bool>>(
            body: Expression.OrElse(
                left: predicate.Body,
                right: ParameterReplacer.Replace(predicate.Parameters.Single(), otherPredicate).Body),
            predicate.Parameters);
    }

    private class ParameterReplacer : ExpressionVisitor
    {
        private readonly ParameterExpression _replacement;

        public ParameterReplacer(ParameterExpression replacement) => _replacement = replacement;

        public static TExpression Replace<TExpression>(ParameterExpression replacement, TExpression expression)
            where TExpression : Expression
        {
            return (TExpression)new ParameterReplacer(replacement).Visit(expression);
        }

        protected override Expression VisitParameter(ParameterExpression parameter)
        {
            return base.VisitParameter(_replacement);
        }
    }
}
