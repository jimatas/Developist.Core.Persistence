using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class DependencyInjectionTests : TestClassBase
{
    [TestMethod]
    public void AddUnitOfWork_ByDefault_RegistersUnitOfWork()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        // Act
        var unitOfWork = serviceProvider.GetService<IUnitOfWorkBase>();

        // Assert
        Assert.IsNotNull(unitOfWork);
    }

    [TestMethod]
    public void AddUnitOfWork_GivenNullRepositoryFactoryType_ThrowsArgumentNullException()
    {
        // Arrange
        Type repositoryFactoryType = null!;

        // Act
        void action() => ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>(repositoryFactoryType));

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(repositoryFactoryType), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(typeof(string))]
    [DataRow(typeof(IRepositoryFactory<>))]
    [DataRow(typeof(IRepositoryFactory<SampleDbContext>))]
    public void AddUnitOfWork_GivenInvalidRepositoryFactoryType_ThrowsArgumentException(Type repositoryFactoryType)
    {
        // Arrange

        // Act
        void action() => ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>(repositoryFactoryType));

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"The provided type '{repositoryFactoryType.Name}' must be a concrete implementation of the 'IRepositoryFactory<TContext>' interface."));
    }

    [TestMethod]
    public void AddUnitOfWork_GivenInvalidGenericRepositoryFactoryType_ThrowsArgumentException()
    {
        var repositoryFactoryType = typeof(RepositoryFactory<>);

        // Act
        void action() => ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>(repositoryFactoryType));

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"The provided generic type '{repositoryFactoryType.Name}' must have a generic parameter of type 'SampleDbContext'."));
    }

    [DataTestMethod]
    [DataRow(typeof(RepositoryFactory<SampleDbContext>))]
    [DataRow(typeof(SampleRepositoryFactory))]
    public void AddUnitOfWork_GivenValidRepositoryFactoryType_DoesNotThrowException(Type repositoryFactoryType)
    {
        // Arrange

        // Act
        ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>(repositoryFactoryType));

        // Assert
    }
}
