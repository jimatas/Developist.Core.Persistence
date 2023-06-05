using System.Linq.Expressions;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for building include paths for entities.
/// </summary>
public static class IncludePathsBuilderExtensions
{
    /// <summary>
    /// Specifies a property to include in the query for the given entity type and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includePaths">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity and property types.</returns>
    public static IIncludePathsBuilder<T, TProperty> Include<T, TProperty>(
        this IIncludePathsBuilder<T> includePaths,
        Expression<Func<T, TProperty>> propertySelector)
    {
        includePaths.Include(IncludePathExtractor.GetPath(propertySelector));

        return new IncludePathsBuilder<T, TProperty>(includePaths.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query for the given entity type, previous property type, and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includePaths">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and property types.</returns>
    public static IIncludePathsBuilder<T, TProperty> Include<T, TPreviousProperty, TProperty>(
        this IIncludePathsBuilder<T, TPreviousProperty> includePaths,
        Expression<Func<T, TProperty>> propertySelector)
    {
        includePaths.Include(IncludePathExtractor.GetPath(propertySelector));

        return includePaths as IIncludePathsBuilder<T, TProperty>
            ?? new IncludePathsBuilder<T, TProperty>(includePaths.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query as a subsequent level in the include path for the given entity type, previous property type, and property type.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of property to include.</typeparam>
    /// <param name="includePaths">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and property types.</returns>
    public static IIncludePathsBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludePathsBuilder<T, TPreviousProperty> includePaths,
        Expression<Func<TPreviousProperty, TProperty>> propertySelector)
    {
        includePaths.ThenInclude(IncludePathExtractor.GetPath(propertySelector));

        return includePaths as IIncludePathsBuilder<T, TProperty>
            ?? new IncludePathsBuilder<T, TProperty>(includePaths.AsList());
    }

    /// <summary>
    /// Specifies a property to include in the query as a subsequent level in the include path for the given entity type, previous property type, and collection of property types.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to include paths.</typeparam>
    /// <typeparam name="TPreviousProperty">The type of the previously included property.</typeparam>
    /// <typeparam name="TProperty">The type of the collection of properties to include.</typeparam>
    /// <param name="includePaths">The builder for constructing include paths.</param>
    /// <param name="propertySelector">The selector for the property to include.</param>
    /// <returns>The builder for constructing include paths for the given entity, previous property, and collection of property types.</returns>
    public static IIncludePathsBuilder<T, TProperty> ThenInclude<T, TPreviousProperty, TProperty>(
        this IIncludePathsBuilder<T, IEnumerable<TPreviousProperty>> includePaths,
        Expression<Func<TPreviousProperty, TProperty>> propertySelector)
    {
        includePaths.ThenInclude(IncludePathExtractor.GetPath(propertySelector));

        return includePaths as IIncludePathsBuilder<T, TProperty>
            ?? new IncludePathsBuilder<T, TProperty>(includePaths.AsList());
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
