using Developist.Core.Persistence.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContext, new()
        {
            return services.AddUnitOfWork<TDbContext>(typeof(RepositoryFactory<TDbContext>), lifetime);
        }

        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, IRepositoryFactory<TDbContext> repositoryFactory, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContext, new()
        {
            ArgumentNullExceptionHelper.ThrowIfNull(() => repositoryFactory);

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TDbContext>), _ => repositoryFactory, lifetime));
            services.AddCommonServices<TDbContext>(lifetime);

            return services;
        }

        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, Type repositoryFactoryType, ServiceLifetime lifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContext, new()
        {
            EnsureValidRepositoryFactoryType();

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TDbContext>), repositoryFactoryType, lifetime));
            services.AddCommonServices<TDbContext>(lifetime);

            return services;

            void EnsureValidRepositoryFactoryType()
            {
                ArgumentNullExceptionHelper.ThrowIfNull(() => repositoryFactoryType);

                if (!repositoryFactoryType.ImplementsGenericInterface(typeof(IRepositoryFactory<>)) || !repositoryFactoryType.IsConcrete())
                {
                    var message = $"Parameter '{nameof(repositoryFactoryType)}' must be a concrete type that implements the {nameof(IRepositoryFactory<TDbContext>)}<{nameof(TDbContext)}> interface.";
                    throw new ArgumentException(message, paramName: nameof(repositoryFactoryType));
                }
            }
        }

        private static IServiceCollection AddCommonServices<TDbContext>(this IServiceCollection services, ServiceLifetime lifetime)
            where TDbContext : DbContext, new()
        {
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork<TDbContext>), typeof(UnitOfWork<TDbContext>), lifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider => provider.GetRequiredService<IUnitOfWork<TDbContext>>(), lifetime));
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), provider => provider.GetRequiredService<IRepositoryFactory<TDbContext>>(), lifetime));
            services.AddDbContext<TDbContext>(contextLifetime: lifetime, optionsLifetime: lifetime);

            return services;
        }
    }
}
