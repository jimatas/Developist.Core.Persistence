using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for <see cref="DbContext"/> objects.
/// </summary>
internal static class DbContextExtensions
{
    /// <summary>
    /// Gets the <see cref="EntityEntry{TEntity}"/> for the specified <paramref name="entity"/>, and attaches it to the <paramref name="dbContext"/> if it is detached and <paramref name="attachIfDetached"/> is true.
    /// </summary>
    /// <typeparam name="T">The type of entity being tracked.</typeparam>
    /// <param name="dbContext">The <see cref="DbContext"/> that is tracking the entity.</param>
    /// <param name="entity">The entity to get the <see cref="EntityEntry{TEntity}"/> for.</param>
    /// <param name="attachIfDetached">A flag indicating whether the entity should be attached to the context if it is detached.</param>
    /// <returns>The <see cref="EntityEntry{TEntity}"/> for the specified entity.</returns>
    public static EntityEntry Entry<T>(this DbContext dbContext, T entity, bool attachIfDetached) where T : class
    {
        var entry = dbContext.Entry(entity);
        if (entry.State == EntityState.Detached && attachIfDetached)
        {
            dbContext.Set<T>().Attach(entity);
        }

        return entry;
    }

    /// <summary>
    /// Validates all entities that have been added or modified in the <paramref name="dbContext"/>.
    /// </summary>
    /// <remarks>If multiple validation errors occur, an <see cref="AggregateException"/> containing the individual <see cref="ValidationException"/> objects will be thrown instead.</remarks>
    /// <param name="dbContext">The <see cref="DbContext"/> to validate entities in.</param>
    /// <exception cref="ValidationException">Thrown when a validation error occurs.</exception>
    public static void ValidateChangedEntities(this DbContext dbContext)
    {
        var exceptions = new List<Exception>();
        foreach (var entity in dbContext.ChangeTracker.Entries().Where(HasChanged).Select(entry => entry.Entity))
        {
            var errors = new List<ValidationResult>();
            if (!Validator.TryValidateObject(entity, new ValidationContext(entity, serviceProvider: null, items: null), errors, validateAllProperties: true))
            {
                exceptions.AddRange(errors.Select(error => new ValidationException(error.ErrorMessage)));
            }
        }

        if (exceptions.Any())
        {
            throw exceptions.Count == 1
                ? exceptions.Single()
                : new AggregateException(exceptions);
        }

        static bool HasChanged(EntityEntry entry) => entry.State is EntityState.Added or EntityState.Modified;
    }
}
