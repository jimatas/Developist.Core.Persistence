using Developist.Core.Persistence.Entities;

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    public static class SortingPaginatorExtensions
    {
        public static SortingPaginator<TEntity> StartingAtPage<TEntity>(this SortingPaginator<TEntity> paginator, int pageNumber)
            where TEntity : IEntity
        {
            paginator.PageNumber = pageNumber;
            return paginator;
        }

        public static SortingPaginator<TEntity> WithPageSizeOf<TEntity>(this SortingPaginator<TEntity> paginator, int pageSize)
            where TEntity : IEntity
        {
            paginator.PageSize = pageSize;
            return paginator;
        }

        public static SortingPaginator<TEntity> SortedByProperty<TEntity>(this SortingPaginator<TEntity> paginator, string propertyName, SortDirection direction = SortDirection.Ascending)
            where TEntity : IEntity
        {
            paginator.SortProperties.Add(new SortProperty<TEntity>(propertyName, direction));
            return paginator;
        }

        public static SortingPaginator<TEntity> SortedByProperty<TEntity, TProperty>(this SortingPaginator<TEntity> paginator, Expression<Func<TEntity, TProperty>> property, SortDirection direction = SortDirection.Ascending)
            where TEntity : IEntity
        {
            paginator.SortProperties.Add(new SortProperty<TEntity, TProperty>(property, direction));
            return paginator;
        }
    }
}
