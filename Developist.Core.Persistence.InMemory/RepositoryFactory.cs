// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System;

namespace Developist.Core.Persistence.InMemory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork uow)
            where TEntity : class, IEntity
        {
            Ensure.Argument.NotNull(uow, nameof(uow));

            var repositoryType = GetRepositoryImplementationType<TEntity>();
            var repository = Activator.CreateInstance(repositoryType, new[] { uow }) as IRepository<TEntity>;

            return repository;
        }

        protected virtual Type GetRepositoryImplementationType<TEntity>()
            where TEntity : class, IEntity => typeof(Repository<TEntity>);
    }
}
