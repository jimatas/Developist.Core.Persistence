// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.InMemory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork uow) where TEntity:class,IEntity
        {
            return new Repository<TEntity>(uow);
        }
    }
}
