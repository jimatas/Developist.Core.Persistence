// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence.Samples
{
    internal class CustomRepositoryFactory : InMemory.RepositoryFactory
    {
        public CustomRepositoryFactory(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override Type GetRepositoryImplementationType<TEntity>() => typeof(CustomRepository<TEntity>);

        private class CustomRepository<TEntity> : InMemory.Repository<TEntity> where TEntity : class, IEntity
        {
            public CustomRepository(IUnitOfWork uow) : base(uow) { }

            public override void Add(TEntity entity)
            {
                base.Add(entity);
            }

            public override void Remove(TEntity entity)
            {
                base.Remove(entity);
            }
        }
    }
}
