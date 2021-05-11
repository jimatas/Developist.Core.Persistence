// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Starting point for specifying include paths for an entity.
    /// </summary>
    public static class EntityIncludePaths
    {
        /// <summary>
        /// Creates a new <see cref="IEntityIncludePaths{TEntity}"/> instance targeting the <typeparamref name="TEntity"/> type.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to include related data for.</typeparam>
        /// <returns>A new <see cref="IEntityIncludePaths{TEntity}"/> object to add the include paths to.</returns>
        public static IEntityIncludePaths<TEntity> ForEntity<TEntity>() where TEntity : IEntity => new IncludePaths<TEntity>();

        /// <summary>
        /// Specify a property include path using a lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to include related data for.</typeparam>
        /// <typeparam name="TProperty">The type of the related data to include.</typeparam>
        /// <param name="includePaths">The source <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object to add the include path to.</param>
        /// <param name="propertySelector">A lambda expression that selects the navigation property to include.</param>
        /// <returns>A new <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object containing both the previous include paths and the newly added one.</returns>
        public static IEntityIncludePaths<TEntity, TProperty> Include<TEntity, TProperty>(this IEntityIncludePaths<TEntity> includePaths, Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : IEntity
        {
            return new IncludePaths<TEntity, TProperty>(includePaths.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        /// <summary>
        /// Specify a property include path using a lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entiy to include related data for.</typeparam>
        /// <typeparam name="TPreviousProperty">The type of the related data that was just included.</typeparam>
        /// <typeparam name="TProperty">The type of the related data to include.</typeparam>
        /// <param name="includePaths">The source <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object to add the include path to.</param>
        /// <param name="propertySelector">A lambda expression that selects the navigation property to include.</param>
        /// <returns>A new <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object containing both the previous include paths and the newly added one.</returns>
        public static IEntityIncludePaths<TEntity, TProperty> Include<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, TPreviousProperty> includePaths, Expression<Func<TEntity, TProperty>> propertySelector) where TEntity : IEntity
        {
            return new IncludePaths<TEntity, TProperty>(includePaths.ToList()) { IncludePathExtractor.Extract(propertySelector) };
        }

        /// <summary>
        /// Further specify a property include path using a lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entiy to include related data for.</typeparam>
        /// <typeparam name="TPreviousProperty">The type of the related data that was just included.</typeparam>
        /// <typeparam name="TProperty">The type of the related data to include.</typeparam>
        /// <param name="includePaths">The source <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object to add the include path to.</param>
        /// <param name="propertySelector">A lambda expression that selects the navigation property to include.</param>
        /// <returns>A new <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object containing both the previous include paths and the newly added one.</returns>
        public static IEntityIncludePaths<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, TPreviousProperty> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector) where TEntity : IEntity
        {
            includePaths.Add(IncludePathExtractor.Extract(propertySelector));

            return new IncludePaths<TEntity, TProperty>(includePaths.ToList());
        }

        /// <summary>
        /// Further specify a property include path using a lambda expression.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entiy to include related data for.</typeparam>
        /// <typeparam name="TPreviousProperty">The type of the related data that was just included.</typeparam>
        /// <typeparam name="TProperty">The type of the related data to include.</typeparam>
        /// <param name="includePaths">The source <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object to add the include path to.</param>
        /// <param name="propertySelector">A lambda expression that selects the navigation property to include.</param>
        /// <returns>A new <see cref="IEntityIncludePaths{TEntity, TProperty}"/> object containing both the previous include paths and the newly added one.</returns>
        public static IEntityIncludePaths<TEntity, TProperty> ThenInclude<TEntity, TPreviousProperty, TProperty>(this IEntityIncludePaths<TEntity, IEnumerable<TPreviousProperty>> includePaths, Expression<Func<TPreviousProperty, TProperty>> propertySelector) where TEntity : IEntity
        {
            includePaths.Add(IncludePathExtractor.Extract(propertySelector));

            return new IncludePaths<TEntity, TProperty>(includePaths.ToList());
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
                Ensure.Argument.NotNull(paths, nameof(paths));

                this.paths = paths is not List<string> || paths.IsReadOnly
                    ? new List<string>(paths)
                    : (List<string>)paths;
            }

            public void Add(string path)
            {
                Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

                paths.Add(path);
            }

            public void Remove(string path)
            {
                Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

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
                Ensure.Argument.NotNullOrWhiteSpace(path, nameof(path));

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
