using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Entities.IncludePaths
{
    public static class IncludePathsBuilderExtensions
    {
        public static IIncludePathsBuilder<TEntity, TProperty> Include<TEntity, TProperty>(this IIncludePathsBuilder<TEntity> includePaths, Expression<Func<TEntity, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            includePaths.Include(IncludePathExtractor.GetPath(propertySelector));
            return new IncludePathsBuilder<TEntity, TProperty>(includePaths.AsList());
        }

        public static IIncludePathsBuilder<TEntity, TProperty> Include<TEntity, TPreviousProperty, TProperty>(this IIncludePathsBuilder<TEntity, TPreviousProperty> includePaths, Expression<Func<TEntity, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            includePaths.Include(IncludePathExtractor.GetPath(propertySelector));
            return includePaths as IIncludePathsBuilder<TEntity, TProperty> ?? new IncludePathsBuilder<TEntity, TProperty>(includePaths.AsList());
        }

        public static IIncludePathsBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludePathsBuilder<TEntity, TPreviousProperty> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            includePaths.ThenInclude(IncludePathExtractor.GetPath(propertySelector));
            return includePaths as IIncludePathsBuilder<TEntity, TProperty> ?? new IncludePathsBuilder<TEntity, TProperty>(includePaths.AsList());
        }

        public static IIncludePathsBuilder<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludePathsBuilder<TEntity, IEnumerable<TPreviousProperty>> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            includePaths.ThenInclude(IncludePathExtractor.GetPath(propertySelector));
            return includePaths as IIncludePathsBuilder<TEntity, TProperty> ?? new IncludePathsBuilder<TEntity, TProperty>(includePaths.AsList());
        }

        private class IncludePathExtractor : ExpressionVisitor
        {
            public string Path { get; private set; } = default!;

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
}
