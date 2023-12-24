using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for working with Entity Framework Core's <see cref="DbContext"/>.
/// </summary>
internal static class DbContextExtensions
{
    /// <summary>
    /// Gets the <see cref="EntityEntry"/> for the specified entity, optionally attaching it if it is detached.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/> instance.</param>
    /// <param name="entity">The entity for which to get the <see cref="EntityEntry"/>.</param>
    /// <param name="attachIfDetached"><see langword="true"/> to attach the entity if it is detached; otherwise, <see langword="false"/>.</param>
    /// <returns>The <see cref="EntityEntry"/> for the specified entity.</returns>
    public static EntityEntry Entry<T>(this DbContext dbContext, T entity, bool attachIfDetached)
        where T : class
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached && attachIfDetached)
        {
            dbContext.Set<T>().Attach(entity);
        }

        return entry;
    }
}
