// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tester
{
    public class Startup
    {
        public Startup(IConfiguration configuration) => Configuration = configuration;

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.Add(new ServiceDescriptor(typeof(IRepository<>), typeof(InMemory.Repository<>), ServiceLifetime.Scoped));
            //services.Add(new ServiceDescriptor(typeof(IReadOnlyRepository<>), provider => provider.GetRequiredService(typeof(IRepository<>)), ServiceLifetime.Scoped));

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), typeof(InMemory.RepositoryFactory), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(InMemory.UnitOfWork), ServiceLifetime.Scoped));
        }
    }
}
