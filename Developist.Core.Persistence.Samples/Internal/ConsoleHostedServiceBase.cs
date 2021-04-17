// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.


using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Samples
{
    #region Sample Program.cs
    // public class Program : ConsoleHostedServiceBase
    // {
    //     public Program(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider, IConfiguration configuration, ILogger<Program> logger)
    //         : base(applicationLifetime, serviceProvider, configuration, logger) { }
    //
    //         private static async Task Main(string[] args) => await CreateHostBuilder(args).RunConsoleAsync();
    //         private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).UseStartup<Startup>();
    //        
    //         protected override async Task OnApplicationStartedAsync(CancellationToken cancellationToken)
    //         {
    //             // Perform work here...
    //         }
    //     }
    // }
    #endregion
    internal abstract class ConsoleHostedServiceBase : IHostedService
    {
        private readonly IHostApplicationLifetime applicationLifetime;
        private readonly IServiceProvider serviceProvider;
        private int? exitCode;

        protected ConsoleHostedServiceBase(IHostApplicationLifetime applicationLifetime, IServiceProvider serviceProvider, IConfiguration configuration, ILogger logger)
        {
            this.applicationLifetime = applicationLifetime ?? throw new ArgumentNullException(nameof(applicationLifetime));
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            Logger = logger ?? NullLogger.Instance;
        }

        protected IServiceProvider ServiceProvider { get; private set; }
        protected IConfiguration Configuration { get; }
        protected ILogger Logger { get; }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            applicationLifetime.ApplicationStarted.Register(() =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        using var scope = serviceProvider.CreateScope();
                        ServiceProvider = scope.ServiceProvider;

                        await OnApplicationStartedAsync(cancellationToken);

                        exitCode = 0;
                    }
                    catch (Exception exception)
                    {
                        Logger.LogWarning(exception, "Exception thrown during application execution.");

                        exitCode = exception.HResult;
                        throw;
                    }
                    finally
                    {
                        applicationLifetime.StopApplication();
                    }
                });
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Environment.ExitCode = exitCode ?? -1;

            return Task.CompletedTask;
        }

        protected abstract Task OnApplicationStartedAsync(CancellationToken cancellationToken);
    }
}
