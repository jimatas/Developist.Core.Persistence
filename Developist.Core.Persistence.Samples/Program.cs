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
            var uow = ServiceProvider.GetRequiredService<IUnitOfWork>();
            if (!uow.IsTransactional)
            {
                await uow.BeginTransactionAsync(cancellationToken);
            }

            IRepository<Person> repository = uow.Repository<Person>();

            new DataSeeder().Seed(repository);
            await uow.CompleteAsync(cancellationToken).ConfigureAwait(true);

            var person = repository.Find(new FilterByName { FamilyName = "Welsh" }).SingleOrDefault();

            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2).SortedBy("Contact.HomeAddress.State", SortDirection.Descending);
            do
            {
                var people = repository.All(paginator);

            } while (paginator.MoveNextPage());
        }
    }
}
