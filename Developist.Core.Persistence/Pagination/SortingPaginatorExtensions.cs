using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
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
        
        public static SortingPaginator<TEntity> SortedBy<TEntity>(this SortingPaginator<TEntity> paginator, string sortString)
            where TEntity : IEntity
        {
            ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => sortString);

            foreach (var property in ReadSortPropertiesFromString())
            {
                paginator.SortProperties.Add(property);
            }
            return paginator;

            IEnumerable<SortProperty<TEntity>> ReadSortPropertiesFromString()
            {
                foreach (var directive in sortString.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                {
                    var propertyName = directive;

                    bool descending;
                    if ((descending = directive.StartsWith('-')) || directive.StartsWith('+'))
                    {
                        propertyName = directive[1..].TrimStart('(').TrimEnd(')');
                    }
                    var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                    SortProperty<TEntity> property;
                    try
                    {
                        property = new SortProperty<TEntity>(propertyName, direction);
                    }
                    catch (ArgumentException exception)
                    {
                        throw new FormatException("Error reading one or more sorting directives from the input string. See the inner exception for details.", exception);
                    }

                    yield return property;
                }
            }
        }
    }
}
