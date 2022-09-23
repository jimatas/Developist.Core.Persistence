using Developist.Core.Persistence.Entities;

using System.Collections.Generic;

namespace Developist.Core.Persistence.Pagination
{
    internal static class ReadOnlyListExtensions
    {
        public static IReadOnlyPaginatedList<TEntity> ToPaginatedList<TEntity>(this IReadOnlyList<TEntity> list, SortingPaginator<TEntity> paginator)
            where TEntity : IEntity
        {
            return new ReadOnlyPaginatedList<TEntity>(list, paginator.PageNumber, paginator.PageSize, paginator.PageCount, paginator.ItemCount);
        }
    }
}
