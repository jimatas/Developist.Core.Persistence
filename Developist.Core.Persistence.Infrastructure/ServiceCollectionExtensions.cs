// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default in-memory <see cref="IUnitOfWork"/> and <see cref="IRepositoryFactory"/> implementations with the built-in dependency injection container.
        /// </summary>
        /// <param name="services"></param>
        /// <param name="repositoryFactoryType">An optionally specified <see cref="Type"/> denoting a custom <see cref="IRepositoryFactory"/> implementation to use.</param>
        /// <param name="lifetime">The lifetime with which to register the services.</param>
        /// <returns></returns>
        public static IServiceCollection AddPersistence(this IServiceCollection services, Type repositoryFactoryType = null, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), EnsureRepositoryFactoryType() ?? typeof(InMemory.RepositoryFactory), lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(InMemory.UnitOfWork), lifetime));
            return services;

            Type EnsureRepositoryFactoryType()
            {
                if (repositoryFactoryType is not null && !(repositoryFactoryType.IsAssignableTo(typeof(IRepositoryFactory)) && repositoryFactoryType.IsConcrete()))
                {
                    throw new ArgumentException($"Parameter '{nameof(repositoryFactoryType)}' must be a conrete type that derives from {nameof(IRepositoryFactory)}.", nameof(repositoryFactoryType));
                }
                return repositoryFactoryType;
            }
        }

        /// <summary>
        /// Registers the default Entity Framework-specific <see cref="IUnitOfWork"/> and <see cref="IRepositoryFactory"/> implementations with the built-in dependency injection container.
        /// </summary>
        /// <typeparam name="TDbContext">The type of the underlying DbContext.</typeparam>
        /// <param name="services"></param>
        /// <param name="repositoryFactoryType">An optionally specified <see cref="Type"/> denoting a custom <see cref="EntityFramework.IRepositoryFactory{TDbContext}"/> implementation to use.</param>
        /// <param name="lifetime">The lifetime with which to register the services.</param>
        /// <returns></returns>
        public static IServiceCollection AddPersistence<TDbContext>(this IServiceCollection services, Type repositoryFactoryType = null, ServiceLifetime lifetime = ServiceLifetime.Scoped) where TDbContext : DbContext
        {
            services.Add(new ServiceDescriptor(typeof(EntityFramework.IRepositoryFactory<TDbContext>), EnsureRepositoryFactoryType() ?? typeof(EntityFramework.RepositoryFactory<TDbContext>), lifetime));
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), provider => provider.GetService<EntityFramework.IRepositoryFactory<TDbContext>>(), lifetime));
            services.Add(new ServiceDescriptor(typeof(EntityFramework.IUnitOfWork<TDbContext>), typeof(EntityFramework.UnitOfWork<TDbContext>), lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider => provider.GetService<EntityFramework.IUnitOfWork<TDbContext>>(), lifetime));
            return services;

            Type EnsureRepositoryFactoryType()
            {
                if (repositoryFactoryType is not null && !(repositoryFactoryType.DerivesFromGenericParent(typeof(EntityFramework.IRepositoryFactory<>)) && repositoryFactoryType.IsConcrete()))
                {
                    throw new ArgumentException($"Parameter '{nameof(repositoryFactoryType)}' must be a concrete type that derives from {nameof(EntityFramework.IRepositoryFactory<TDbContext>)}<{nameof(TDbContext)}>.", nameof(repositoryFactoryType));
                }
                return repositoryFactoryType;
            }
        }
    }
}
