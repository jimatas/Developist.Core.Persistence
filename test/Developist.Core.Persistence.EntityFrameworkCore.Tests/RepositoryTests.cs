using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class RepositoryTests : TestClassBase
{
    [TestMethod]
    public void Initialize_WithNullUnitOfWork_ThrowsArgumentNullException()
    {
        // Arrange
        IUnitOfWork unitOfWork = null!;

        // Act
        void action() => _ = new Repository<Person>(unitOfWork);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(unitOfWork), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(false, false)]
    [DataRow(true, true)]
    public async Task AnyAsync_ByDefault_ReturnsExpectedResult(bool seeded, bool expectedResult)
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        if (seeded)
        {
            await repository.InitializedWithPeopleAsync();
        }

        // Act
        var result = await repository.AnyAsync();

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task AnyAsync_GivenNullFilterCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person> filterCriteria = null!;

        // Act
        Task action() => repository.AnyAsync(filterCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(filterCriteria), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("ll", true, true)]
    [DataRow("Hollie", false, true)]
    [DataRow("John", false, false)]
    public async Task AnyAsync_GivenFilterCriteria_ReturnsExpectedResult(string givenName, bool usePartialMatching, bool expectedResult)
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        var filterCriteria = new PredicateFilterCriteria<Person>(
            p => usePartialMatching
                ? p.GivenName.Contains(givenName)
                : p.GivenName.Equals(givenName));

        // Act
        var result = await repository.AnyAsync(filterCriteria);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task CountAsync_ByDefault_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var count = await repository.CountAsync();

        // Assert
        Assert.AreEqual(9, count);
    }

    [TestMethod]
    public async Task CountAsync_GivenNullFilterCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person> filterCriteria = null!;

        // Act
        Task action() => repository.CountAsync(filterCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(filterCriteria), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("ll", true, 3)]
    [DataRow("Hollie", false, 1)]
    [DataRow("John", false, 0)]
    public async Task CountAsync_GivenFilterCriteria_ReturnsExpectedResult(string givenName, bool usePartialMatching, int expectedResult)
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        var filterCriteria = new PredicateFilterCriteria<Person>(
            p => usePartialMatching
                ? p.GivenName.Contains(givenName)
                : p.GivenName.Equals(givenName));

        // Act
        var result = await repository.CountAsync(filterCriteria);

        // Assert
        Assert.AreEqual(expectedResult, result);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_ByDefault_ReturnsExpectedResult()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(result);

        var expectedResult = People.AsEnumerable().First();
        Assert.AreEqual(expectedResult.ToString(), result.ToString());
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_CalledOnEmptyRepository_ReturnsDefaultValue()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        var result = await repository.FirstOrDefaultAsync();

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_GivenNullFilterCriteria_ThrowsArgumentNullException()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person> filterCriteria = null!;

        // Act
        Task action() => repository.FirstOrDefaultAsync(filterCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(filterCriteria), exception.ParamName);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_GivenFilterCriteria_ReturnsExpectedResult()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var filterCriteria = new PredicateFilterCriteria<Person>(p => p.FamilyName.Equals("Connor"));

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.FirstOrDefaultAsync(filterCriteria);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Phillipa Connor", result.ToString());
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_GivenNullFilterCriteria_ThrowsArgumentNullException()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person> filterCriteria = null!;

        // Act
        Task action() => repository.SingleOrDefaultAsync(filterCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(filterCriteria), exception.ParamName);
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_GivenFilterCriteriaReturningMultipleResults_ThrowsInvalidOperationException()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var filterCriteria = new PredicateFilterCriteria<Person>(p => p.FamilyName.Equals("Connor"));

        await repository.InitializedWithPeopleAsync();

        // Act
        Task action() => repository.SingleOrDefaultAsync(filterCriteria);

        // Assert
        await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_GivenFilterCriteriaReturningNoResult_ReturnsDefaultValue()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var filterCriteria = new PredicateFilterCriteria<Person>(p => p.GivenName.Equals("John") && p.FamilyName.Equals("Doe"));

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.SingleOrDefaultAsync(filterCriteria);

        // Assert
        Assert.IsNull(result);
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_GivenFilterCriteria_ReturnsExpectedResult()
    {
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var filterCriteria = new PredicateFilterCriteria<Person>(p => p.GivenName.Equals("Phillipa") && p.FamilyName.Equals("Connor"));

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.SingleOrDefaultAsync(filterCriteria);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Phillipa Connor", result.ToString());
    }

    [TestMethod]
    public async Task ListAsync_GivenNullPaginationCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IPaginationCriteria<Person> paginationCriteria = null!;

        // Act
        Task action() => repository.ListAsync(paginationCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(paginationCriteria), exception.ParamName);
    }

    [TestMethod]
    public async Task ListAsync_GivenNullFilterCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IFilterCriteria<Person> filterCriteria = null!;
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        Task action() => repository.ListAsync(filterCriteria, paginationCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(filterCriteria), exception.ParamName);
    }

    [TestMethod]
    public async Task ListAsync_GivenPaginationCriteria_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 3);

        await repository.InitializedWithPeopleAsync();

        // Act
        var expectedResult = await repository.ListAsync(paginationCriteria);

        // Assert
        Assert.IsNotNull(expectedResult);
        Assert.AreEqual(paginationCriteria.PageSize, expectedResult.Count);
        Assert.AreEqual(paginationCriteria.PageNumber, expectedResult.PageNumber);
        Assert.AreEqual(await repository.CountAsync(), expectedResult.TotalCount);
    }

    [TestMethod]
    public async Task ListAsync_GivenFilterCriteriaAndPaginationCriteria_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        var filterCriteria = new PredicateFilterCriteria<Person>(p => p.FamilyName.Equals("Bloom"));
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 10);

        // Act
        var expectedResult = await repository.ListAsync(filterCriteria, paginationCriteria);

        // Assert
        Assert.AreEqual(1, expectedResult.Count);
        Assert.AreEqual("Randall", expectedResult[0].GivenName);
    }
}
