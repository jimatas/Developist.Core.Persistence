using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Developist.Core.Persistence.InMemory
{
    internal static class QueryableExtensions
    {
        public static IReadOnlyList<TEntity> ToReadOnlyList<TEntity>(this IQueryable<TEntity> query)
        {
            return new ReadOnlyCollection<TEntity>(query.ToList());
        }
    }
}
