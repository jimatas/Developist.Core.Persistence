using Developist.Core.Persistence.Utilities;

using System.Collections;
using System.Collections.Generic;

namespace Developist.Core.Persistence.Pagination
{
    public class ReadOnlyPaginatedList<T> : IReadOnlyPaginatedList<T>
    {
        public ReadOnlyPaginatedList(IReadOnlyList<T> innerList, int pageNumber, int pageSize, int pageCount, int itemCount)
        {
            InnerList = ArgumentNullExceptionHelper.ThrowIfNull(() => innerList);
            PageNumber = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => pageNumber, minValue: 1);
            PageSize = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => pageSize, minValue: 1);
            PageCount = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => pageCount, minValue: 0);
            ItemCount = ArgumentOutOfRangeExceptionHelper.ThrowIfOutOfRange(() => itemCount, minValue: 0);
        }

        protected IReadOnlyList<T> InnerList { get; }

        public int PageNumber { get; }
        public int PageSize { get; }
        public int PageCount { get; }
        public int ItemCount { get; }

        public int Count => InnerList.Count;
        public T this[int index] => InnerList[index];
        public IEnumerator<T> GetEnumerator() => InnerList.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
