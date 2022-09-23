using System.Linq.Expressions;

namespace Developist.Core.Persistence.Utilities
{
    internal static class ExpressionExtensions
    {
        public static string? GetMemberName(this LambdaExpression expression)
        {
            return (expression.Body as MemberExpression ?? (expression.Body as UnaryExpression)?.Operand as MemberExpression)?.Member.Name;
        }
    }
}
