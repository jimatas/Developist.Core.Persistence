using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

public abstract class TestClassBase
{
    protected static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
    {
        var services = new ServiceCollection();
        configureServices(services);
        return services.BuildServiceProvider();
    }
}
