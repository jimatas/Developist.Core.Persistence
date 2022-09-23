using Microsoft.EntityFrameworkCore;

using System.Collections.ObjectModel;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    internal static class QueryableExtensions
    {
        public static async Task<IReadOnlyList<TEntity>> ToReadOnlyListAsync<TEntity>(this IQueryable<TEntity> query, CancellationToken cancellationToken = default)
        {
            return new ReadOnlyCollection<TEntity>(await query.ToListAsync(cancellationToken).ConfigureAwait(false));
        }
    }
}
