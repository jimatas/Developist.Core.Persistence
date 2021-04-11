// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity;

        event EventHandler<UnitOfWorkEventArgs> Completed;
        void Complete();
        Task CompleteAsync(CancellationToken cancellationToken = default);
    }
}
