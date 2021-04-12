// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;

namespace Developist.Core.Persistence.Samples
{
    internal static class HostBuilderExtensions
    {
        public static IHostBuilder UseStartup<TStartup>(this IHostBuilder hostBuilder) where TStartup : class =>
            hostBuilder.ConfigureServices((context, services) =>
            {
                var hasConfigurationConstructor = typeof(TStartup).GetConstructor(new[] { typeof(IConfiguration) }) is not null;
                var startupInstance = hasConfigurationConstructor
                    ? (TStartup)Activator.CreateInstance(typeof(TStartup), context.Configuration)
                    : (TStartup)Activator.CreateInstance(typeof(TStartup));

                var configureServicesMethod = typeof(TStartup).GetMethod("ConfigureServices", new[] { typeof(IServiceCollection) });
                configureServicesMethod?.Invoke(startupInstance, new[] { services });
            });
    }
}
