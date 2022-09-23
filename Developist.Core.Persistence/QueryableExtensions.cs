using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Utilities;

using System.Linq;

namespace Developist.Core.Persistence
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> Filter<TEntity>(this IQueryable<TEntity> query, IQueryableFilter<TEntity> filter)
            where TEntity : IEntity
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => filter);
            return filter.Filter(query);
        }

        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> query, IQueryablePaginator<TEntity> paginator)
            where TEntity : IEntity
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => paginator);
            return paginator.Paginate(query);
        }
    }
}
