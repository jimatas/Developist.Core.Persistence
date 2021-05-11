﻿// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.InMemory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider serviceProvider;

        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = Ensure.Argument.NotNull(serviceProvider, nameof(serviceProvider));
        }

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork uow) where TEntity : class, IEntity
        {
            Ensure.Argument.NotNull(uow, nameof(uow));

            var factory = ActivatorUtilities.CreateFactory(GetRepositoryImplementationType<TEntity>(), new[] { uow.GetType() });
            return (IRepository<TEntity>)factory(serviceProvider, new[] { uow });
        }

        /// <summary>
        /// Override this method in order to return a custom <see cref="IRepository{TEntity}"/> implementation type with which entities of the specified generic type are persisted.
        /// </summary>
        /// <typeparam name="TEntity">The entitity type for which to return a repository type.</typeparam>
        /// <returns>A repository type for the specified entity type.</returns>
        protected virtual Type GetRepositoryImplementationType<TEntity>() where TEntity : class, IEntity
            => typeof(Repository<TEntity>);
    }
}
