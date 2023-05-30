using Microsoft.Extensions.DependencyInjection;
using System;

namespace Developist.Core.Persistence.InMemory.DependencyInjection
{
    /// <summary>
    /// Provides extension methods for configuring the in-memory implementation of the unit of work pattern.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the default <see cref="UnitOfWork"/> and <see cref="RepositoryFactory"/> implementations with the specified service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="lifetime">The lifetime of the services. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            return services.AddUnitOfWork(typeof(RepositoryFactory), lifetime);
        }

        /// <summary>
        /// Registers the specified <see cref="IRepositoryFactory"/> and default <see cref="UnitOfWork"/> implementations with the specified service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="repositoryFactory">The <see cref="IRepositoryFactory"/> implementation to use.</param>
        /// <param name="lifetime">The lifetime of the services. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, IRepositoryFactory repositoryFactory, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (repositoryFactory is null)
            {
                throw new ArgumentNullException(nameof(repositoryFactory));
            }

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), _ => repositoryFactory, lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), lifetime));

            return services;
        }

        /// <summary>
        /// Registers the specified <see cref="IRepositoryFactory"/> type and default <see cref="UnitOfWork"/> implementations with the specified service lifetime.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="repositoryFactoryType">The <see cref="IRepositoryFactory"/> type to use.</param>
        /// <param name="lifetime">The lifetime of the services. Defaults to <see cref="ServiceLifetime.Scoped"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance for chaining.</returns>
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, Type repositoryFactoryType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            if (repositoryFactoryType is null)
            {
                throw new ArgumentNullException(nameof(repositoryFactoryType));
            }

            if (!typeof(IRepositoryFactory).IsAssignableFrom(repositoryFactoryType) || !repositoryFactoryType.IsConcrete())
            {
                throw new ArgumentException(
                    message: $"The provided type '{repositoryFactoryType.Name}' must be a concrete implementation of the '{nameof(IRepositoryFactory)}' interface.",
                    paramName: nameof(repositoryFactoryType));
            }

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), repositoryFactoryType, lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), lifetime));

            return services;
        }
    }
}
