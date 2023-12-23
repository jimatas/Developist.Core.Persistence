using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class UnitOfWorkTests : TestClassBase
{
    [TestMethod]
    public void Repository_GivenEntityType_ReturnsRepositoryForThatEntityType()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();

        // Act
        var repository = unitOfWork.Repository<Person>();

        // Assert
        Assert.IsNotNull(repository);
    }

    [TestMethod]
    public void Repository_CalledTwiceForSameEntityType_ReturnsSameRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();

        // Act
        var repository = unitOfWork.Repository<Person>();
        var otherRepository = unitOfWork.Repository<Person>();

        // Assert
        Assert.AreSame(repository, otherRepository);
    }

    [TestMethod]
    public void Repository_CalledTwiceForDifferentEntityTypes_ReturnsDifferentRepositories()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();

        // Act
        var bookRepository = unitOfWork.Repository<Book>();
        var personRepository = unitOfWork.Repository<Person>();

        // Assert
        Assert.AreNotEqual<object>(bookRepository, personRepository);
    }

    [TestMethod]
    public async Task CompleteAsync_ByDefault_RaisesCompletedEvent()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();

        var isCompleted = false;
        unitOfWork.Completed += (sender, e) => isCompleted = true;

        // Act
        await unitOfWork.CompleteAsync();

        // Assert
        Assert.IsTrue(isCompleted);
    }

    [TestMethod]
    public async Task CompleteAsync_ByDefault_SetsCorrectUnitOfWorkArgument()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();

        IUnitOfWorkBase? actualUnitOfWork = null;
        unitOfWork.Completed += (sender, e) => actualUnitOfWork = e.UnitOfWork;

        // Act
        await unitOfWork.CompleteAsync();

        // Assert
        Assert.IsNotNull(actualUnitOfWork);
        Assert.AreSame(unitOfWork, actualUnitOfWork);
    }
}
