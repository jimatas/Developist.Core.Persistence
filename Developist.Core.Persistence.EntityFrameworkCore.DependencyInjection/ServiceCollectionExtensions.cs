// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork<TDbContext>(this IServiceCollection services, Type customRepositoryFactoryType = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
            where TDbContext : DbContext, new()
        {
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork<TDbContext>), typeof(UnitOfWork<TDbContext>), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), provider => provider.GetRequiredService<IUnitOfWork<TDbContext>>(), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory<TDbContext>), EnsureRepositoryFactoryType() ?? typeof(RepositoryFactory<TDbContext>), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), provider => provider.GetRequiredService<IRepositoryFactory<TDbContext>>(), serviceLifetime));
            services.AddDbContext<TDbContext>(contextLifetime: serviceLifetime, optionsLifetime: serviceLifetime);

            return services;

            Type EnsureRepositoryFactoryType()
            {
                if (customRepositoryFactoryType is not null && !(customRepositoryFactoryType.ImplementsGenericInterface(typeof(IRepositoryFactory<>)) && customRepositoryFactoryType.IsConcrete()))
                {
                    var message = $"Parameter '{nameof(customRepositoryFactoryType)}' must be a concrete type that implements the {nameof(IRepositoryFactory<TDbContext>)}<{nameof(TDbContext)}> interface.";
                    throw new ArgumentException(message, paramName: nameof(customRepositoryFactoryType));
                }
                return customRepositoryFactoryType;
            }
        }
    }
}
