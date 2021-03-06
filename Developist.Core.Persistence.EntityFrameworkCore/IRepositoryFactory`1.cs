// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public interface IRepositoryFactory<TDbContext> : IRepositoryFactory
        where TDbContext : DbContext
    {
        IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> uow)
            where TEntity : class, IEntity;
    }
}
