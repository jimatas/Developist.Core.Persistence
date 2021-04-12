// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        event EventHandler<UnitOfWorkCompletedEventArgs> Completed;

        void Complete();
        Task CompleteAsync(CancellationToken cancellationToken = default);
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;
    }
}
