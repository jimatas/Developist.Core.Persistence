using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <inheritdoc/>
public static partial class RepositoryExtensions
{
    /// <summary>
    /// Creates a new instance of the repository that performs queries without tracking entity changes.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <returns>A new repository instance with tracking disabled.</returns>
    public static IRepository<T> WithoutTracking<T>(this IRepository<T> repository) where T : class
    {
        return repository.ExtendWithFeature(
            queryExtender: new NoTrackingQueryExtender<T>(),
            featureName: "tracking entities");
    }

    // Declared as internal for unit test visibility.
    internal class NoTrackingQueryExtender<T> : IQueryExtender<T> where T : class
    {
        public IQueryable<T> Extend(IQueryable<T> query)
        {
            return query.AsNoTracking();
        }
    }
}
