// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Samples
{
    class Program : ConsoleHostedServiceBase
    {
        #region Startup
        public Program(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider, IConfiguration configuration, ILogger<Program> logger)
            : base(applicationLifetime, serviceProvider, configuration, logger) { }

        public static async Task Main(string[] args) => await CreateHostBuilder(args).RunConsoleAsync();
        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).UseStartup<Startup>();
        #endregion

        protected async override Task OnApplicationStartedAsync(CancellationToken cancellationToken)
        {
            using var uow = ServiceProvider.GetRequiredService<IUnitOfWork>();

            new DataSeeder().Seed(uow.Repository<Person>());
            await uow.CompleteAsync(cancellationToken);

            var person = uow.Repository<Person>().Find(p => p.FamilyName.Contains("Welsh")).SingleOrDefault();
        }
    }
}
