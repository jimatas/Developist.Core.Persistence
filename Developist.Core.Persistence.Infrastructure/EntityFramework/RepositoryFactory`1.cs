// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFramework
{
    public class RepositoryFactory<TDbContext> : IRepositoryFactory<TDbContext> where TDbContext : DbContext
    {
        IRepository<TEntity> IRepositoryFactory.Create<TEntity>(IUnitOfWork uow) => Create<TEntity>((IUnitOfWork<TDbContext>)uow);

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> uow) where TEntity : class, IEntity
        {
            return new Repository<TEntity, TDbContext>(uow);
        }
    }
}
