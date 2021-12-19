// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        bool IsTransactional { get; }

        void BeginTransaction();
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        void Complete();
        Task CompleteAsync(CancellationToken cancellationToken = default);

        IRepository<TEntity> Repository<TEntity>()
            where TEntity : class, IEntity;
    }
}
