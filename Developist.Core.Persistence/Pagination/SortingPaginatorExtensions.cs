﻿using Developist.Core.Persistence.Entities;
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

        public static SortingPaginator<T> SortedByString<T>(this SortingPaginator<T> paginator, string value)
            where T : IEntity
        {
            ArgumentExceptionHelper.ThrowIfNullOrWhiteSpace(() => value);

            foreach (var property in ParseSortPropertiesFromString())
            {
                paginator.SortProperties.Add(property);
            }
            return paginator;

            IEnumerable<SortProperty<T>> ParseSortPropertiesFromString()
            {
                foreach (var directive in value.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
                {
                    var propertyName = directive;

                    bool descending;
                    if ((descending = directive.StartsWith('-')) || directive.StartsWith('+'))
                    {
                        propertyName = directive[1..].TrimStart('(').TrimEnd(')');
                    }

                    var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                    SortProperty<T> property;
                    try
                    {
                        property = new SortProperty<T>(propertyName, direction);
                    }
                    catch (ArgumentException exception)
                    {
                        throw new FormatException("Error parsing one or more sorting directives from the input string. See the inner exception for details.", exception);
                    }

                    yield return property;
                }
            }
        }
    }
}
