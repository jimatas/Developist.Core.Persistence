// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>(IUnitOfWork uow) where TEntity : class, IEntity;
    }
}
