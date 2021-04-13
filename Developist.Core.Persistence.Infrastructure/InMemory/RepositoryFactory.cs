// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.InMemory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork uow) where TEntity : class, IEntity
        {
            if (uow is null)
            {
                throw new ArgumentNullException(nameof(uow));
            }

            var factory = ActivatorUtilities.CreateFactory(typeof(Repository<TEntity>), new[] { uow.GetType() });
            return (IRepository<TEntity>)factory(serviceProvider, new[] { uow });
        }
    }
}
