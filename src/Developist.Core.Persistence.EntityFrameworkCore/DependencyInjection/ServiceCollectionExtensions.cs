using Developist.Core.Persistence;
using Developist.Core.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to add unit of work and repository services to the dependency injection container.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds a unit of work and repository services to the dependency injection container for the specified database context type.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <param name="lifetime">The lifetime of the services.</param>
    /// <returns>The collection of services.</returns>
    public static IServiceCollection AddUnitOfWork<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped) where TContext : DbContext
    {
        return services.AddUnitOfWork<TContext>(typeof(RepositoryFactory<TContext>), lifetime);
    }

    /// <summary>
    /// Adds a unit of work and repository services to the dependency injection container for the specified database context type and repository factory.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <param name="repositoryFactory">The repository factory used to create repositories.</param>
    /// <param name="lifetime">The lifetime of the services.</param>
    /// <returns>The collection of services.</returns>
    public static IServiceCollection AddUnitOfWork<TContext>(
        this IServiceCollection services,
        IRepositoryFactory<TContext> repositoryFactory,
        ServiceLifetime lifetime = ServiceLifetime.Scoped) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(repositoryFactory);

        services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TContext>), _ => repositoryFactory, lifetime));
        services.AddCommonServices<TContext>(lifetime);

        return services;
    }

    /// <summary>
    /// Adds a unit of work and repository services to the dependency injection container for the specified database context type and repository factory type.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <param name="repositoryFactoryType">The type of the repository factory used to create repositories.</param>
    /// <param name="lifetime">The lifetime of the services.</param>
    /// <returns>The collection of services.</returns>
    public static IServiceCollection AddUnitOfWork<TContext>(
        this IServiceCollection services,
        Type repositoryFactoryType,
        ServiceLifetime lifetime = ServiceLifetime.Scoped) where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(repositoryFactoryType);

        if (!repositoryFactoryType.GetImplementedGenericInterfaces(typeof(IRepositoryFactory<>)).Any() || !repositoryFactoryType.IsConcrete())
        {
            throw new ArgumentException(
                message: $"The provided type '{repositoryFactoryType.Name}' must be a concrete implementation of the '{nameof(IRepositoryFactory<TContext>)}<{nameof(TContext)}>' interface.",
                paramName: nameof(repositoryFactoryType));
        }

        services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TContext>), repositoryFactoryType, lifetime));
        services.AddCommonServices<TContext>(lifetime);

        return services;
    }

    private static IServiceCollection AddCommonServices<TContext>(this IServiceCollection services, ServiceLifetime lifetime)
        where TContext : DbContext
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork<TContext>), typeof(UnitOfWork<TContext>), lifetime));
        services.Add(new ServiceDescriptor(typeof(IUnitOfWorkBase), provider => provider.GetRequiredService<IUnitOfWork<TContext>>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider => provider.GetRequiredService<IUnitOfWork<TContext>>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), provider => provider.GetRequiredService<IRepositoryFactory<TContext>>(), lifetime));
        services.AddDbContext<TContext>(contextLifetime: lifetime, optionsLifetime: lifetime);

        return services;
    }
}
