// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Tester.Utilities;

using Microsoft.Extensions.Hosting;

using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tester
{
    class Program
    {
        public static async Task Main(string[] args) => await CreateHostBuilder(args).RunConsoleAsync();
        private static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args).UseStartup<Startup>();
    }
}
