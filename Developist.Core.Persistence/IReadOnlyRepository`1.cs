// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IReadOnlyRepository<TEntity> where TEntity : IEntity
    {
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes);

        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IEntityIncludePaths<TEntity> includes, CancellationToken cancellationToken = default);
    }
}
