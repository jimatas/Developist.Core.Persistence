using Developist.Core.Persistence.Entities;

using System.Linq;

namespace Developist.Core.Persistence
{
    public interface IQueryableFilter<TEntity>
        where TEntity : IEntity
    {
        IQueryable<TEntity> Filter(IQueryable<TEntity> query);
    }
}
