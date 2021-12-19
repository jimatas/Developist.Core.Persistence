// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Entities
{
    public static class IncludePathCollectionExtensions
    {
        public static IIncludePathCollection<TEntity, TProperty> Include<TEntity, TProperty>(this IIncludePathCollection<TEntity> includePaths, Expression<Func<TEntity, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            return new IncludePathCollection<TEntity, TProperty>(includePaths.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        public static IIncludePathCollection<TEntity, TProperty> Include<TEntity, TPreviousProperty, TProperty>(this IIncludePathCollection<TEntity, TPreviousProperty> includePaths, Expression<Func<TEntity, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            return new IncludePathCollection<TEntity, TProperty>(includePaths.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        public static IIncludePathCollection<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludePathCollection<TEntity, TPreviousProperty> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            IIncludePathCollection<TEntity, TProperty> newIncludePaths = new IncludePathCollection<TEntity, TProperty>(includePaths.ToList());
            newIncludePaths.Add(IncludePathExtractor.Extract(propertySelector));
            return newIncludePaths;
        }

        public static IIncludePathCollection<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IIncludePathCollection<TEntity, IEnumerable<TPreviousProperty>> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector)
            where TEntity : IEntity
        {
            IIncludePathCollection<TEntity, TProperty> newIncludePaths = new IncludePathCollection<TEntity, TProperty>(includePaths.ToList());
            newIncludePaths.Add(IncludePathExtractor.Extract(propertySelector));
            return newIncludePaths;
        }

        private class IncludePathExtractor : ExpressionVisitor
        {
            public string Path { get; private set; }

            public static string Extract(Expression propertySelector)
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
