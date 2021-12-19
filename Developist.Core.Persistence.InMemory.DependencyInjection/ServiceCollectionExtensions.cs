// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence.InMemory.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services, Type customRepositoryFactoryType = null, ServiceLifetime serviceLifetime = ServiceLifetime.Scoped)
        {
            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), EnsureRepositoryFactoryType() ?? typeof(RepositoryFactory), serviceLifetime));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(UnitOfWork), serviceLifetime));

            return services;

            Type EnsureRepositoryFactoryType()
            {
                if (customRepositoryFactoryType != null && !(typeof(IRepositoryFactory).IsAssignableFrom(customRepositoryFactoryType) && customRepositoryFactoryType.IsConcrete()))
                {
                    var message = $"Parameter '{nameof(customRepositoryFactoryType)}' must be a concrete type that implements the {nameof(IRepositoryFactory)} interface.";
                    throw new ArgumentException(message, paramName: nameof(customRepositoryFactoryType));
                }
                return customRepositoryFactoryType;
            }
        }
    }
}
