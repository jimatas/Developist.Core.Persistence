using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.IntegrationTests.Fixture;
using Developist.Core.Persistence.IntegrationTests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.IntegrationTests;

[TestClass]
public class UnitOfWorkTests
{
    [TestMethod]
    public async Task BeginTransactionAsync_CalledTwice_ThrowsInvalidOperationException()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();
        await unitOfWork.BeginTransactionAsync();

        // Act
        var action = async () => await unitOfWork.BeginTransactionAsync();

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        Assert.AreEqual("An active transaction is already in progress. "
            + "This operation does not support nested transactions. "
            + "Please complete the current unit of work before beginning a new transaction.", exception.Message);
    }
}
