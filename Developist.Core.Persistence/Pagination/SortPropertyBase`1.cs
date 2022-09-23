using Developist.Core.Persistence.Entities;

using System.Linq;

namespace Developist.Core.Persistence.Pagination
{
    public abstract class SortPropertyBase<TEntity>
        where TEntity : IEntity
    {
        protected SortPropertyBase(SortDirection direction) => Direction = direction;

        public SortDirection Direction { get; }

        public abstract IOrderedQueryable<TEntity> Sort(IQueryable<TEntity> query);
    }
}
