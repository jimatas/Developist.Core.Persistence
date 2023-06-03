using Developist.Core.Persistence.Tests.Fixture;
using Developist.Core.Persistence.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class UnitOfWorkTests
{
    [TestMethod]
    public void Repository_GivenType_ReturnsRepositoryForThatType()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var repository = unitOfWork.Repository<Person>();

        // Assert
        Assert.IsNotNull(repository);
    }

    [TestMethod]
    public void Repository_CalledTwiceForSameType_ReturnsSameRepository()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var repository = unitOfWork.Repository<Person>();
        var anotherRepository = unitOfWork.Repository<Person>();

        // Assert
        Assert.AreEqual(repository, anotherRepository);
    }

    [TestMethod]
    public void Repository_CalledTwiceForDifferentTypes_ReturnsDifferentRepositories()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var bookRepository = unitOfWork.Repository<Book>();
        var personRepository = unitOfWork.Repository<Person>();

        // Assert
        Assert.AreNotEqual<object>(bookRepository, personRepository);
    }

    [TestMethod]
    public async Task CompleteAsync_ByDefault_FiresCompletedEvent()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        bool isCompleted = false;
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
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        IUnitOfWork? sourceUnitOfWork = null;
        unitOfWork.Completed += (sender, e) => sourceUnitOfWork = e.UnitOfWork;

        // Act
        await unitOfWork.CompleteAsync();

        // Assert
        Assert.IsNotNull(sourceUnitOfWork);
        Assert.AreEqual(unitOfWork, sourceUnitOfWork);
    }

    [TestMethod]
    public async Task BeginTransactionAsync_CalledOnInMemoryUnitOfWork_DoesNotStartTransaction()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        // Act
        var hasActiveTransactionBeforeCall = unitOfWork.HasActiveTransaction;
        await unitOfWork.BeginTransactionAsync();
        var hasActiveTransactionAfterCall = unitOfWork.HasActiveTransaction;

        // Assert
        Assert.IsFalse(hasActiveTransactionBeforeCall);
        Assert.AreEqual(hasActiveTransactionBeforeCall, hasActiveTransactionBeforeCall);
    }
}
