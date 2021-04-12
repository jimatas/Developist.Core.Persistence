// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : class, IEntity
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TEntity entity);
        void Remove(TEntity entity);
    }
}
