using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Provides extension methods for the <see cref="IRepository{T}"/> interface.
    /// </summary>
    public static class RepositoryExtensions
    {
        /// <summary>
        /// Counts the number of entities in the repository that satisfy a predicate.
        /// </summary>
        /// <typeparam name="T">The type of entity in the repository.</typeparam>
        /// <param name="repository">The repository to count entities from.</param>
        /// <param name="predicate">The predicate used to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns the number of entities that satisfy the predicate.</returns>
        public static Task<int> CountAsync<T>(this IRepository<T> repository,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return repository.CountAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
        }

        /// <summary>
        /// Retrieves a single entity from the repository that satisfies the specified predicate, throwing an exception if multiple entities match the predicate.
        /// </summary>
        /// <typeparam name="T">The type of entity in the repository.</typeparam>
        /// <param name="repository">The repository to retrieve the entity from.</param>
        /// <param name="predicate">The predicate used to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns the single entity that satisfies the predicate, or <c>null</c> if no entity is found.</returns>
        public static Task<T> SingleOrDefaultAsync<T>(this IRepository<T> repository,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return repository.SingleOrDefaultAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
        }

        /// <summary>
        /// Retrieves the first entity from the repository that satisfies the specified predicate.
        /// </summary>
        /// <typeparam name="T">The type of entity in the repository.</typeparam>
        /// <param name="repository">The repository to retrieve the entity from.</param>
        /// <param name="predicate">The predicate used to filter entities.</param>
        /// <param name="cancellationToken">The cancellation token used to cancel the operation.</param>
        /// <returns>A task that represents the asynchronous operation and returns the first entity that satisfies the predicate, or <c>null</c> if no entity is found.</returns>
        public static Task<T> FirstOrDefaultAsync<T>(this IRepository<T> repository,
            Expression<Func<T, bool>> predicate,
            CancellationToken cancellationToken = default)
        {
            return repository.FirstOrDefaultAsync(new PredicateFilterCriteria<T>(predicate), cancellationToken);
        }
    }
}
