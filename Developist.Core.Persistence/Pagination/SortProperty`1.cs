using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Utilities;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    public class SortProperty<TEntity> : SortPropertyBase<TEntity>
        where TEntity : IEntity
    {
        public SortProperty(string propertyName, SortDirection direction)
            : base(direction)
        {
            ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => propertyName);
            Property = GetPropertySelector(propertyName);
        }

        public LambdaExpression Property { get; }

        public override IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> query)
        {
            var sortMethodName = query.IsSorted()
                ? Direction == SortDirection.Ascending
                    ? "ThenBy"
                    : "ThenByDescending"
                : Direction == SortDirection.Ascending
                    ? "OrderBy"
                    : "OrderByDescending";

            var sortMethod = typeof(Queryable).GetMethods().Single(method => method.Name.Equals(sortMethodName) && method.GetParameters().Length == 2);
            sortMethod = sortMethod.MakeGenericMethod(typeof(TEntity), Property.ReturnType);

            return (IOrderedQueryable<TEntity>)sortMethod.Invoke(null, new object[] { query, Property });
        }

        private static LambdaExpression GetPropertySelector(string propertyName)
        {
            var type = typeof(TEntity);
            var parameter = Expression.Parameter(type, "p");

            Expression expression = parameter;
            foreach (var nestedProperty in propertyName.Split('.'))
            {
                var property = type.GetProperty(nestedProperty);
                if (property is null)
                {
                    throw new ArgumentException($"No property '{nestedProperty}' on type '{type.Name}'.", nameof(propertyName));
                }

                expression = Expression.Property(expression, property);
                type = property.PropertyType;
            }

            expression = Expression.Lambda(expression, parameter);
            return (LambdaExpression)expression;
        }
    }
}
