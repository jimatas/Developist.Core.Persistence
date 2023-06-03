using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines the interface for paginating data using an <see cref="IQueryable{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of data being paginated.</typeparam>
    public interface IPaginator<T>
    {
        /// <summary>
        /// Paginates the specified <see cref="IQueryable{T}"/> and returns an <see cref="IPaginatedList{T}"/> containing the results.
        /// </summary>
        /// <param name="query">The <see cref="IQueryable{T}"/> to paginate.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A task representing the asynchronous operation. The result of the task is an <see cref="IPaginatedList{T}"/> containing the paginated results.</returns>
        Task<IPaginatedList<T>> PaginateAsync(IQueryable<T> query, CancellationToken cancellationToken = default);
    }
}
