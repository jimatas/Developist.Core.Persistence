using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a query builder implementation that configures a query for non-tracking behavior.
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
internal class NonTrackingQueryBuilder<T> : IQueryBuilder<T> where T : class
{
    /// <inheritdoc/>
    public IQueryable<T> BuildQuery(IQueryable<T> query)
    {
        return query.AsNoTracking();
    }
}
