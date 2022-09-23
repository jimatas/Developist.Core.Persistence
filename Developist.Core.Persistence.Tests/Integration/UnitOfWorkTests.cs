using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests.Integration
{
    [TestClass]
    public class UnitOfWorkTests
    {
        private const string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database=Developist_Core_Persistence_IntegrationTests;Trusted_Connection=true;MultipleActiveResultSets=true";

        [TestMethod]
        public async Task BeginTransactionAsync_CalledTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();
            await unitOfWork.BeginTransactionAsync();

            // Act
            async Task action() => await unitOfWork.BeginTransactionAsync();

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        }

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            IServiceCollection services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }
    }
}
