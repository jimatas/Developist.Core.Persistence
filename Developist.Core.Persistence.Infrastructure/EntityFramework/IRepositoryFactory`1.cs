// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFramework
{
    public interface IRepositoryFactory<TDbContext> : IRepositoryFactory where TDbContext : DbContext
    {
        IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> uow) where TEntity : class, IEntity;
    }
}
