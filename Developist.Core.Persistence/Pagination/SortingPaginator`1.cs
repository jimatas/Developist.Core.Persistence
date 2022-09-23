using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Pagination
{
    public class SortingPaginator<TEntity> : IQueryablePaginator<TEntity>
        where TEntity : IEntity
    {
        public const int DefaultPageSize = 20;

        private int pageNumber;
        private int pageSize;
        private int itemCount;

        public SortingPaginator() : this(pageNumber: 1) { }
        public SortingPaginator(int pageNumber) : this(pageNumber, DefaultPageSize) { }
        public SortingPaginator(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int PageNumber
        {
            get => pageNumber;
            set => pageNumber = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(value, nameof(PageNumber), minValue: 1);
        }

        public int PageSize
        {
            get => pageSize;
            set => pageSize = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(value, nameof(PageSize), minValue: 1);
        }

        public int PageCount { get; private set; }

        public int ItemCount
        {
            get => itemCount;
            private set
            {
                itemCount = value;
                PageCount = (int)Math.Ceiling((double)itemCount / PageSize);
            }
        }

        public ICollection<SortPropertyBase<TEntity>> SortProperties { get; } = new List<SortPropertyBase<TEntity>>();

        public IQueryable<TEntity> Paginate(IQueryable<TEntity> query)
        {
            ItemCount = query.Count();

            IOrderedQueryable<TEntity>? sortedQuery = null;
            foreach (var directive in SortProperties)
            {
                sortedQuery = directive.Sort(sortedQuery ?? query);
            }

            if (sortedQuery != null)
            {
                query = sortedQuery.Skip((PageNumber - 1) * PageSize);
            }
            query = query.Take(PageSize);

            return query;
        }
    }
}
