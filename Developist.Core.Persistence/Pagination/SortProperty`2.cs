using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Utilities;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    public class SortProperty<TEntity, TProperty> : SortPropertyBase<TEntity>
        where TEntity : IEntity
    {
        public SortProperty(Expression<Func<TEntity, TProperty>> property, SortDirection direction)
            : base(direction)
        {
            Property = ArgumentNullExceptionHelper.ThrowIfNull(() => property);
        }

        public Expression<Func<TEntity, TProperty>> Property { get; }

        public override IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> query)
        {
            return query.IsSorted()
                ? Direction == SortDirection.Ascending
                    ? ((IOrderedQueryable<TEntity>)query).ThenBy(Property)
                    : ((IOrderedQueryable<TEntity>)query).ThenByDescending(Property)
                : Direction == SortDirection.Ascending
                    ? query.OrderBy(Property)
                    : query.OrderByDescending(Property);
        }
    }
}
