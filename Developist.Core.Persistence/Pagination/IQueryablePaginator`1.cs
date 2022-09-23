using Developist.Core.Persistence.Entities;

using System.Linq;

namespace Developist.Core.Persistence.Pagination
{
    public interface IQueryablePaginator<TEntity>
        where TEntity : IEntity
    {
        IQueryable<TEntity> Paginate(IQueryable<TEntity> query);
    }
}
