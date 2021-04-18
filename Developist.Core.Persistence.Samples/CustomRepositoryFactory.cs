// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Logging;

using System;

namespace Developist.Core.Persistence.Samples
{
    internal class CustomRepositoryFactory : InMemory.RepositoryFactory
    {
        public CustomRepositoryFactory(IServiceProvider serviceProvider) : base(serviceProvider) { }

        protected override Type GetRepositoryImplementationType<TEntity>() => typeof(CustomRepository<TEntity>);

        private class CustomRepository<TEntity> : InMemory.Repository<TEntity> where TEntity : class, IEntity
        {
            private readonly ILogger<CustomRepository<TEntity>> logger;

            public CustomRepository(IUnitOfWork uow, ILogger<CustomRepository<TEntity>> logger = null) : base(uow)
            {
                this.logger = logger;
            }

            public override void Add(TEntity entity)
            {
                logger.LogInformation($"Adding entity: {entity}");
                base.Add(entity);
            }

            public override void Remove(TEntity entity)
            {
                logger.LogInformation($"Removing entity: {entity}");
                base.Remove(entity);
            }
        }
    }
}
