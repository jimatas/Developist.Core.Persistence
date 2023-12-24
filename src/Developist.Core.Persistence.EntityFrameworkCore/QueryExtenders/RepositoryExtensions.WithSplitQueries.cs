using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <inheritdoc/>
public static partial class RepositoryExtensions
{
    /// <summary>
    /// Creates a new instance of the repository that utilizes split queries for data retrieval operations.
    /// </summary>
    /// <typeparam name="T">The type of entity in the repository.</typeparam>
    /// <param name="repository">The original repository instance.</param>
    /// <returns>A new repository instance with query splitting enabled.</returns>
    public static IRepository<T> WithSplitQueries<T>(this IRepository<T> repository) where T : class
    {
        return repository.ExtendWithFeature(
            queryExtender: new SplitQueryExtender<T>(),
            featureName: "query splitting");
    }

    // Declared as internal for unit test visibility.
    internal class SplitQueryExtender<T> : IQueryExtender<T> where T : class
    {
        public IQueryable<T> Extend(IQueryable<T> query)
        {
            return query.AsSplitQuery();
        }
    }
}
