// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.EntityFramework
{
    public class RepositoryFactory<TDbContext> : IRepositoryFactory<TDbContext> where TDbContext : DbContext
    {
        private readonly IServiceProvider serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        IRepository<TEntity> IRepositoryFactory.Create<TEntity>(IUnitOfWork uow) => Create<TEntity>((IUnitOfWork<TDbContext>)uow);

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> uow) where TEntity : class, IEntity
        {
            var factory = ActivatorUtilities.CreateFactory(typeof(Repository<TEntity, TDbContext>), new[] { uow.GetType() });
            return (IRepository<TEntity>)factory(serviceProvider, new[] { uow });
        }
    }
}
