using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// A paginator that supports sorting by multiple properties.
    /// </summary>
    /// <typeparam name="T">The type of the items being paged.</typeparam>
    public class SortingPaginator<T> : IPaginator<T>
    {
        /// <summary>
        /// The default number of items to include in a single page of a paginated list.
        /// </summary>
        public const int DefaultPageSize = 20;

        private int _pageNumber;
        private int _pageSize;
        private int _itemCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class with the default page number (1) and page size (20).
        /// </summary>
        public SortingPaginator() : this(pageNumber: 1) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class with the specified page number and the default page size (20).
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve.</param>
        public SortingPaginator(int pageNumber) : this(pageNumber, DefaultPageSize) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="SortingPaginator{T}"/> class with the specified page number and page size.
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve.</param>
        /// <param name="pageSize">The number of items to include in each page.</param>
        public SortingPaginator(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        /// <summary>
        /// Gets or sets the number of the page to retrieve.
        /// </summary>
        public int PageNumber
        {
            get => _pageNumber;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(PageNumber), value, message: "Value cannot be less than 1.");
                }

                _pageNumber = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of items to be returned per page.
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(paramName: nameof(PageSize), value, message: "Value cannot be less than 1.");
                }

                _pageSize = value;
            }
        }

        /// <summary>
        /// Gets the total number of pages.
        /// </summary>
        public int PageCount { get; protected set; }

        /// <summary>
        /// Gets the total number of items.
        /// </summary>
        public int ItemCount
        {
            get => _itemCount;
            protected set
            {
                _itemCount = value;
                PageCount = (int)Math.Ceiling((double)_itemCount / PageSize);
            }
        }

        /// <summary>
        /// Gets a collection of <see cref="SortablePropertyBase{T}"/> objects representing the properties by which the results are sorted.
        /// </summary>
        public ICollection<SortablePropertyBase<T>> SortableProperties { get; } = new List<SortablePropertyBase<T>>();

        /// <inheritdoc/>
        public virtual Task<IPaginatedList<T>> PaginateAsync(IQueryable<T> query, CancellationToken cancellationToken = default)
        {
            ItemCount = query.Count();

            IReadOnlyList<T> innerList = PaginateCore(query).ToList();
            IPaginatedList<T> result = new PaginatedList<T>(innerList, PageNumber, PageSize, ItemCount);

            return Task.FromResult(result);
        }

        /// <summary>
        /// Provides the core implementation for paginating a query based on the current page number, page size, and sort properties.
        /// </summary>
        /// <remarks>
        /// This method is meant to be called by <see cref="PaginateAsync"/> implementations or overrides.
        /// </remarks>
        /// <param name="query">The query to paginate.</param>
        /// <returns>An <see cref="IQueryable{T}"/> object that represents the paginated results.</returns>
        protected virtual IQueryable<T> PaginateCore(IQueryable<T> query)
        {
            IOrderedQueryable<T> sortedQuery = null;
            foreach (var property in SortableProperties)
            {
                sortedQuery = (sortedQuery ?? query).SortBy(property);
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
