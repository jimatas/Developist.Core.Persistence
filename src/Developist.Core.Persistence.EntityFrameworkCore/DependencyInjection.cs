using Developist.Core.Persistence;
using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// Provides extension methods to add unit of work and repository services to the dependency injection container.
/// </summary>
public static class DependencyInjection
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
    /// Adds a unit of work and repository services to the dependency injection container for the specified database context type and repository factory type.
    /// </summary>
    /// <typeparam name="TContext">The type of the database context.</typeparam>
    /// <typeparam name="TRepositoryFactory">The type of the repository factory used to create repositories.</typeparam>
    /// <param name="services">The collection of services.</param>
    /// <param name="lifetime">The lifetime of the services.</param>
    /// <returns>The collection of services.</returns>
    public static IServiceCollection AddUnitOfWork<TContext, TRepositoryFactory>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        where TContext : DbContext
        where TRepositoryFactory : IRepositoryFactory<TContext>
    {
        return services.AddUnitOfWork<TContext>(typeof(TRepositoryFactory), lifetime);
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
        Ensure.NotNull(repositoryFactoryType);
        ValidateRepositoryFactoryType<TContext>(repositoryFactoryType);

        services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TContext>), repositoryFactoryType, lifetime));
        services.AddCommonServices<TContext>(lifetime);

        return services;
    }

    private static IServiceCollection AddCommonServices<TContext>(
        this IServiceCollection services,
        ServiceLifetime lifetime) where TContext : DbContext
    {
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork<TContext>), typeof(UnitOfWork<TContext>), lifetime));
        services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider => provider.GetRequiredService<IUnitOfWork<TContext>>(), lifetime));
        services.Add(new ServiceDescriptor(typeof(IUnitOfWorkBase), provider => provider.GetRequiredService<IUnitOfWork<TContext>>(), lifetime));
        services.TryAdd(new ServiceDescriptor(typeof(IRepositoryFactory<TContext>), typeof(RepositoryFactory<TContext>), lifetime));
        services.AddDbContext<TContext>(contextLifetime: lifetime, optionsLifetime: lifetime);

        return services;
    }

    private static void ValidateRepositoryFactoryType<TContext>(Type repositoryFactoryType) where TContext : DbContext
    {
        if (!repositoryFactoryType.IsConcrete() || !repositoryFactoryType.ImplementsGenericInterface(typeof(IRepositoryFactory<>)))
        {
            throw new ArgumentException(
                message: $"The provided type '{repositoryFactoryType.Name}' must be a concrete implementation of the '{nameof(IRepositoryFactory<TContext>)}<{nameof(TContext)}>' interface.",
                paramName: nameof(repositoryFactoryType));
        }

        if (repositoryFactoryType.IsGenericType && !repositoryFactoryType.HasGenericTypeArgument<TContext>())
        {
            throw new ArgumentException(
                message: $"The provided generic type '{repositoryFactoryType.Name}' must have a generic parameter of type '{typeof(TContext).Name}'.",
                paramName: nameof(repositoryFactoryType));
        }
    }
}
