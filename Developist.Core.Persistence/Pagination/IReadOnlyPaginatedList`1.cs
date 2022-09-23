using System.Collections.Generic;

namespace Developist.Core.Persistence.Pagination
{
    public interface IReadOnlyPaginatedList<T> : IReadOnlyList<T>
    {
        int PageNumber { get; }
        int PageSize { get; }
        int PageCount { get; }
        int ItemCount { get; }
    }
}
