using Developist.Core.Persistence.InMemory;
using Developist.Core.Persistence.InMemory.DependencyInjection;
using Developist.Core.Persistence.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class ServiceCollectionExtensionsTests
{
    [TestMethod]
    public void AddUnitOfWork_ByDefault_RegistersUnitOfWork()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());

        // Act
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        // Assert
        Assert.IsNotNull(unitOfWork);
    }

    [TestMethod]
    public void AddUnitOfWork_GivenNullRepositoryFactoryType_ThrowsArgumentNullException()
    {
        // Arrange
        Type repositoryFactoryType = null!;

        // Act
        var action = () => ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork(repositoryFactoryType));

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [DataTestMethod]
    [DataRow(typeof(object))]
    [DataRow(typeof(IRepositoryFactory))]
    public void AddUnitOfWork_GivenInvalidRepositoryFactoryType_ThrowsArgumentException(Type repositoryFactoryType)
    {
        // Arrange

        // Act
        var action = () => ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork(repositoryFactoryType));

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual($"The provided type '{repositoryFactoryType.Name}' must be a concrete implementation of the 'IRepositoryFactory' interface. (Parameter 'repositoryFactoryType')", exception.Message);
    }

    [TestMethod]
    public void AddUnitOfWork_GivenValidRepositoryFactoryType_DoesNotThrow()
    {
        // Arrange
        var repositoryFactoryType = typeof(RepositoryFactory);

        // Act

        // Assert
        ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork(repositoryFactoryType));
    }

    [TestMethod]
    public void AddUnitOfWork_GivenNullRepositoryFactory_ThrowsArgumentNullException()
    {
        // Arrange
        IRepositoryFactory repositoryFactory = null!;

        // Act
        var action = () => ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork(repositoryFactory));

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public void AddUnitOfWork_GivenRepositoryFactory_RegistersIt()
    {
        // Arrange
        var repositoryFactory = new RepositoryFactory(ServiceProviderHelper.ConfigureServiceProvider(_ => { }));
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork(repositoryFactory));

        // Act
        var registeredRepositoryFactory = serviceProvider.GetRequiredService<IRepositoryFactory>();

        // Assert
        Assert.AreEqual(repositoryFactory, registeredRepositoryFactory);
    }
}
