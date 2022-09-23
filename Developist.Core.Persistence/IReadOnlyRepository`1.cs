using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.Pagination;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IReadOnlyRepository<TEntity>
        where TEntity : IEntity
    {
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathsBuilder<TEntity> includePaths, CancellationToken cancellationToken = default);
    }
}
