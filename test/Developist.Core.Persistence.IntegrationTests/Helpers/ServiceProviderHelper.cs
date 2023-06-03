using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.IntegrationTests.Helpers;

internal static class ServiceProviderHelper
{
    public static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);

        return services.BuildServiceProvider();
    }
}
