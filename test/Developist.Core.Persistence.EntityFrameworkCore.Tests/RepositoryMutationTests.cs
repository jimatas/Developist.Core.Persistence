using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class RepositoryMutationTests : TestClassBase
{
    [TestMethod]
    public void Add_GivenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        Person entity = null!;

        // Act
        void action() => repository.Add(entity!);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(entity), exception.ParamName);
    }

    [TestMethod]
    public async Task Add_WithoutCompleteAsync_DoesNotAddEntity()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var entity = new Person { GivenName = "John", FamilyName = "Smith" };

        // Act
        var anyBefore = await repository.AnyAsync();

        repository.Add(entity);

        var anyAfter = await repository.AnyAsync();

        // Assert
        Assert.IsFalse(anyBefore);
        Assert.IsFalse(anyAfter);
    }

    [TestMethod]
    public async Task Add_FollowedByCompleteAsync_AddsEntity()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var entity = new Person { GivenName = "John", FamilyName = "Smith" };

        // Act
        var anyBefore = await repository.AnyAsync();

        repository.Add(entity);
        await unitOfWork.CompleteAsync();

        var anyAfter = await repository.AnyAsync();

        // Assert
        Assert.IsFalse(anyBefore);
        Assert.IsTrue(anyAfter);
    }

    [TestMethod]
    public void Remove_GivenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        Person entity = null!;

        // Act
        void action() => repository.Remove(entity!);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(entity), exception.ParamName);
    }

    [TestMethod]
    public async Task Remove_WithoutCompleteAsync_DoesNotRemoveEntity()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();
        var entity = await repository.FirstOrDefaultAsync();

        // Act
        var countBefore = await repository.CountAsync();

        repository.Remove(entity!);

        var countAfter = await repository.CountAsync();

        // Assert
        Assert.AreEqual(countBefore, countAfter);
    }

    [TestMethod]
    public async Task Remove_FollowedByCompleteAsync_RemovesEntity()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();
        var entity = await repository.FirstOrDefaultAsync();

        // Act
        var countBefore = await repository.CountAsync();

        repository.Remove(entity!);
        await unitOfWork.CompleteAsync();

        var countAfter = await repository.CountAsync();

        // Assert
        Assert.AreNotEqual(countBefore, countAfter);
    }
}
