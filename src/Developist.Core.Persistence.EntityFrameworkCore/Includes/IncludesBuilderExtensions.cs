namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for building include paths for entities.
/// </summary>
public static class IncludesBuilderExtensions
{
    /// <summary>
    /// Specifies a property to include in the query for the given entity type and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includes">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity and property types.</returns>
    public static IIncludesBuilder<T, TProperty> Include<T, TProperty>(
        this IIncludesBuilder<T> includes,
        Expression<Func<T, TProperty>> propertySelector)
    {
        includes.Include(IncludePathExtractor.GetPath(propertySelector));

        return new IncludesBuilder<T, TProperty>(includes.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query for the given entity type, previous property type, and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includes">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and property types.</returns>
    public static IIncludesBuilder<T, TProperty> Include<T, TPreviousProperty, TProperty>(
        this IIncludesBuilder<T, TPreviousProperty> includes,
        Expression<Func<T, TProperty>> propertySelector)
    {
        includes.Include(IncludePathExtractor.GetPath(propertySelector));

        return includes as IIncludesBuilder<T, TProperty>
            ?? new IncludesBuilder<T, TProperty>(includes.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query as a subsequent level in the include path for the given entity type, previous property type, and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includes">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and property types.</returns>
    public static IIncludesBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludesBuilder<T, TPreviousProperty> includes,
        Expression<Func<TPreviousProperty, TProperty>> propertySelector)
    {
        includes.ThenInclude(IncludePathExtractor.GetPath(propertySelector));

        return includes as IIncludesBuilder<T, TProperty>
            ?? new IncludesBuilder<T, TProperty>(includes.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query as a subsequent level in the include path for the given entity type, previous property type, and collection of property types.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of the collection of properties to include.</typeparam>
    /// <param name="includes">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and collection of property types.</returns>
    public static IIncludesBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludesBuilder<T, IEnumerable<TPreviousProperty>> includes,
        Expression<Func<TPreviousProperty, TProperty>> propertySelector)
    {
        includes.ThenInclude(IncludePathExtractor.GetPath(propertySelector));

        return includes as IIncludesBuilder<T, TProperty>
            ?? new IncludesBuilder<T, TProperty>(includes.AsList());
    }

    private class IncludePathExtractor : ExpressionVisitor
    {
        public string Path { get; private set; } = string.Empty;

        public static string GetPath(Expression propertySelector)
        {
            var extractor = new IncludePathExtractor();
            extractor.Visit(propertySelector);

            return extractor.Path;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Path = $"{node.Member.Name}.{Path}".TrimEnd('.');

            return base.VisitMember(node);
        }
    }
}
