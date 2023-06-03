using Developist.Core.Persistence.InMemory;
using Developist.Core.Persistence.Tests.Fixture;
using Developist.Core.Persistence.Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class RepositoryTests
{
    [TestMethod]
    public void Initialize_WithNullUnitOfWork_ThrowsArgumentNullException()
    {
        // Arrange
        IUnitOfWork? unitOfWork = null;

        // Act
        var action = () => new Repository<Person>(unitOfWork);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public async Task CountAsync_ByDefault_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();

        await repository.SeedWithDataAsync();

        // Act
        var count = await repository.CountAsync();

        // Assert
        Assert.AreEqual(9, count);
    }

    [TestMethod]
    public async Task CountAsync_GivenNullCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person>? criteria = null;

        // Act
        var action = () => repository.CountAsync(criteria);

        // Assert
        await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
    }

    [DataTestMethod]
    [DataRow("ll", true, 3)]
    [DataRow("Hollie", false, 1)]
    [DataRow("John", false, 0)]
    public async Task CountAsync_GivenCriteria_ReturnsExpectedResult(string givenName, bool usePartialMatching, int expectedResult)
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();

        await repository.SeedWithDataAsync();

        var criteria = new PersonByNameCriteria
        {
            GivenName = givenName,
            UsePartialMatching = usePartialMatching
        };

        // Act
        var result = await repository.CountAsync(criteria);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task ListAsync_GivenNullPaginator_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        IPaginator<Person>? paginator = null;

        // Act
        var action = () => repository.ListAsync(paginator);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(paginator), exception.ParamName);
    }

    [TestMethod]
    public async Task ListAsync_GivenNullCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person>? criteria = null;
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => repository.ListAsync(criteria, paginator);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(criteria), exception.ParamName);
    }

    [TestMethod]
    public async Task ListAsync_GivenPaginator_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 3);

        await repository.SeedWithDataAsync();

        // Act
        var expectedResult = await repository.ListAsync(paginator);

        // Assert
        Assert.IsNotNull(expectedResult);
        Assert.AreEqual(paginator.PageSize, expectedResult.Count);
        Assert.AreEqual(paginator.PageNumber, expectedResult.PageNumber);
    }

    [TestMethod]
    public async Task ListAsync_GivenCriteriaAndPaginator_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();

        await repository.SeedWithDataAsync();

        var criteria = new PersonByNameCriteria { FamilyName = "Bloom" };
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(10);

        // Act
        var expectedResult = await repository.ListAsync(criteria, paginator);

        // Assert
        Assert.AreEqual(1, expectedResult.Count);
        Assert.AreEqual("Randall", expectedResult.Single().GivenName);
    }

    [TestMethod]
    public void Add_GivenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        Person? entity = null;

        // Act
        var action = () => repository.Add(entity!);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(entity), exception.ParamName);
    }

    [TestMethod]
    public void Add_WithoutCompleteAsync_DoesNotCommit()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = (Repository<Person>)unitOfWork.Repository<Person>();
        var entity = new Person { GivenName = "John", FamilyName = "Smith" };

        // Act
        var countBeforeAdding = repository.DataStore.Count;
        repository.Add(entity);
        var countAfterAdding = repository.DataStore.Count;

        // Assert
        Assert.AreEqual(0, countBeforeAdding);
        Assert.AreEqual(countBeforeAdding, countAfterAdding);
    }

    [TestMethod]
    public async Task Add_WithCompleteAsync_Commits()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = (Repository<Person>)unitOfWork.Repository<Person>();
        var entity = new Person() { GivenName = "John", FamilyName = "Smith" };

        // Act
        var countBeforeAdding = repository.DataStore.Count;

        repository.Add(entity);
        await unitOfWork.CompleteAsync();

        var countAfterAdding = repository.DataStore.Count;

        // Assert
        Assert.AreNotEqual(countBeforeAdding, countAfterAdding);
    }

    [TestMethod]
    public void Remove_GivenNullEntity_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();
        Person? entity = null;

        // Act
        var action = () => repository.Remove(entity!);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(entity), exception.ParamName);
    }

    [TestMethod]
    public async Task Remove_WithoutCompleteAsync_DoesNotCommit()
    {
        // Arrange
        using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = (Repository<Person>)unitOfWork.Repository<Person>();

        await repository.SeedWithDataAsync();

        var criteria = new PersonByNameCriteria { FamilyName = "Hensley" };
        var paginator = new SortingPaginator<Person>();

        var entity = (await repository.ListAsync(criteria, paginator)).Single();

        // Act
        var countBeforeRemoval = repository.DataStore.Count;

        repository.Remove(entity);

        var countAfterRemoval = repository.DataStore.Count;

        // Assert
        Assert.AreEqual(9, countBeforeRemoval);
        Assert.AreEqual(countBeforeRemoval, countAfterRemoval);
    }

    [TestMethod]
    public async Task Remove_WithCompleteAsync_Commits()
    {
        // Arrange
        using var provider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork());
        var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
        var repository = (Repository<Person>)unitOfWork.Repository<Person>();

        await repository.SeedWithDataAsync();

        var criteria = new PersonByNameCriteria { FamilyName = "Hensley" };
        var paginator = new SortingPaginator<Person>();

        var entity = (await repository.ListAsync(criteria, paginator)).Single();

        // Act
        var countBeforeRemoval = repository.DataStore.Count;

        repository.Remove(entity);
        await unitOfWork.CompleteAsync();

        var countAfterRemoval = repository.DataStore.Count;

        // Assert
        Assert.AreNotEqual(countBeforeRemoval, countAfterRemoval);
    }
}
