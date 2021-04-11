// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Entities
{
    public static class EntityIncludePaths
    {
        public static IEntityIncludePaths<TEntity> ForEntity<TEntity>() where TEntity : IEntity => new IncludePaths<TEntity>();

        public static IEntityIncludePaths<TEntity, TProperty> Include<TEntity, TProperty>(this IEntityIncludePaths<TEntity> includes, Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : IEntity
        {
            return new IncludePaths<TEntity, TProperty>(includes.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        public static IEntityIncludePaths<TEntity, TProperty> Include<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, TPreviousProperty> includes, Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : IEntity
        {
            return new IncludePaths<TEntity, TProperty>(includes.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        public static IEntityIncludePaths<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, TPreviousProperty> includes, Expression<Func<TPreviousProperty, TProperty>> propertySelector) where TEntity : IEntity
        {
            includes.Add(IncludePathExtractor.Extract(propertySelector));

            return new IncludePaths<TEntity, TProperty>(includes.ToList());
        }

        public static IEntityIncludePaths<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, IEnumerable<TPreviousProperty>> includes, Expression<Func<TPreviousProperty, TProperty>> propertySelector) where TEntity : IEntity
        {
            includes.Add(IncludePathExtractor.Extract(propertySelector));

            return new IncludePaths<TEntity, TProperty>(includes.ToList());
        }

        #region Nested types
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

        private class IncludePaths<TEntity> : IEntityIncludePaths<TEntity> where TEntity : IEntity
        {
            private readonly List<string> paths;

            public IncludePaths() : this(new List<string>()) { }
            public IncludePaths(IList<string> paths)
            {
                this.paths = paths is not List<string> || paths.IsReadOnly
                    ? new List<string>(paths)
                    : (List<string>)paths;
            }

            public void Add(string path)
            {
                paths.Add(path);
            }

            public void Remove(string path)
            {
                var i = paths.LastIndexOf(path);
                if (i >= 0)
                {
                    paths.RemoveAt(i);
                }
            }

            public IEnumerator<string> GetEnumerator() => paths.GetEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        private class IncludePaths<TEntity, TProperty> : IncludePaths<TEntity>, IEntityIncludePaths<TEntity, TProperty> where TEntity : IEntity
        {
            public IncludePaths() : base() { }
            public IncludePaths(IList<string> paths) : base(paths) { }

            void IEntityIncludePaths<TEntity, TProperty>.Add(string path)
            {
                var previousPath = this.LastOrDefault();
                if (previousPath is not null)
                {
                    Remove(previousPath);
                    path = $"{previousPath}.{path}";
                }

                Add(path);
            }
        }
        #endregion
    }
}
