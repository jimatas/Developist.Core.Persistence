using Developist.Core.Persistence.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.InMemory.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            return services.AddUnitOfWork(typeof(RepositoryFactory), lifetime);
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, IRepositoryFactory repositoryFactory, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => repositoryFactory);

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), _ => repositoryFactory, lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), lifetime));

            return services;
        }

        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, Type repositoryFactoryType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
        {
            EnsureValidRepositoryFactoryType();

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), repositoryFactoryType, lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), lifetime));

            return services;

            void EnsureValidRepositoryFactoryType()
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => repositoryFactoryType);

                if (!typeof(IRepositoryFactory).IsAssignableFrom(repositoryFactoryType) || !repositoryFactoryType.IsConcrete())
                {
                    var message = $"Parameter '{nameof(repositoryFactoryType)}' must be a concrete type that implements the {nameof(IRepositoryFactory)} interface.";
                    throw new ArgumentException(message, paramName: nameof(repositoryFactoryType));
                }
            }
        }
    }
}
