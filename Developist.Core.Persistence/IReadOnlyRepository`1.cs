// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IReadOnlyRepository<TEntity>
        where TEntity : IEntity
    {
        #region Synchronous methods
        int Count();
        int Count(IQueryableFilter<TEntity> filter);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator);
        IEnumerable<TEntity> Find(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths);
        #endregion

        #region Asynchronous methods
        Task<int> CountAsync(CancellationToken cancellationToken = default);
        Task<int> CountAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, CancellationToken cancellationToken = default);
        Task<IEnumerable<TEntity>> FindAsync(IQueryableFilter<TEntity> filter, IQueryablePaginator<TEntity> paginator, IIncludePathCollection<TEntity> includePaths, CancellationToken cancellationToken = default);
        #endregion
    }
}
