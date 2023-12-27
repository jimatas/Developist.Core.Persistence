namespace Developist.Core.Persistence.Pagination;

/// <summary>
/// Provides extension methods for working with property expressions.
/// </summary>
internal static class PropertyExpressionExtensions
{
    /// <summary>
    /// Gets a <see cref="LambdaExpression"/> representing a property selector based on a string property name.
    /// </summary>
    /// <typeparam name="T">The type for which the property selector is created.</typeparam>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A <see cref="LambdaExpression"/> representing the property selector.</returns>
    public static LambdaExpression GetPropertySelector<T>(this string propertyName)
    {
        var type = typeof(T);
        var parameter = Expression.Parameter(type, "p");
        Expression expression = parameter;

        foreach (var nestedPropertyName in propertyName.Split('.'))
        {
            var property = type.GetPropertyIgnoreCase(nestedPropertyName)
                ?? throw new ArgumentException(
                    message: $"No accessible property '{nestedPropertyName}' defined on type '{type.Name}'.",
                    paramName: nameof(propertyName));

            expression = Expression.Property(expression, property);
            type = property.PropertyType;
        }

        return Expression.Lambda(expression, parameter);
    }

    /// <summary>
    /// Gets the name of the property from a property selector <see cref="LambdaExpression"/>.
    /// </summary>
    /// <param name="propertySelector">The property selector <see cref="LambdaExpression"/>.</param>
    /// <returns>The name of the property, or <see langword="null"/> if the property name cannot be determined.</returns>
    public static string? GetPropertyName(this LambdaExpression propertySelector)
    {
        return BuildPropertyPath(propertySelector.Body);
    }

    private static string? BuildPropertyPath(Expression? expression)
    {
        return expression switch
        {
            MemberExpression memberExpression => $"{BuildPropertyPath(memberExpression.Expression)}.{memberExpression.Member.Name}".TrimStart('.'),
            UnaryExpression unaryExpression when unaryExpression.NodeType == ExpressionType.Convert => BuildPropertyPath(unaryExpression.Operand),
            _ => null,
        };
    }
}
