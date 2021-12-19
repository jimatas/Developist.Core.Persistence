// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System;
using System.Data.Entity;

namespace Developist.Core.Persistence.EntityFramework
{
    public class RepositoryFactory<TDbContext> : IRepositoryFactory<TDbContext>
        where TDbContext : DbContext
    {
        IRepository<TEntity> IRepositoryFactory.Create<TEntity>(IUnitOfWork uow) 
            => Create<TEntity>((IUnitOfWork<TDbContext>)uow);

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> uow)
            where TEntity : class, IEntity
        {
            Ensure.Argument.NotNull(uow, nameof(uow));

            var repositoryType = GetRepositoryImplementationType<TEntity>();
            var repository = Activator.CreateInstance(repositoryType, new[] { uow }) as IRepository<TEntity>;

            return repository;
        }

        protected virtual Type GetRepositoryImplementationType<TEntity>()
            where TEntity : class, IEntity => typeof(Repository<TEntity, TDbContext>);
    }
}
