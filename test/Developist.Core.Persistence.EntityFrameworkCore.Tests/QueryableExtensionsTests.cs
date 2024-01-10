using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class QueryableExtensionsTests : TestClassBase
{
    [TestMethod]
    public async Task ToPaginatedListAsync_GivenNullPaginationCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();

        PaginationCriteria<Person> paginationCriteria = null!;
        var dbContext = unitOfWork.DbContext;
        var query = dbContext.Set<Person>().AsQueryable();

        // Act
        Task action() => query.ToPaginatedListAsync(paginationCriteria);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(paginationCriteria), exception.ParamName);
    }

    [TestMethod]
    public async Task ToPaginatedListAsync_GivenValidPaginationCriteria_ReturnsPaginatedList()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, int?>(p => p.Age, SortDirection.Descending));

        var dbContext = unitOfWork.DbContext;
        var query = dbContext.Set<Person>().AsQueryable();

        // Act
        var subset = await query.ToPaginatedListAsync(paginationCriteria);

        // Assert
        Assert.IsNotNull(subset);
        Assert.AreEqual(2, subset.Count);
        Assert.AreEqual(80, subset[0].Age);
        Assert.AreEqual(55, subset[1].Age);
    }

    [TestMethod]
    public async Task ToPaginatedListAsync_GivenConfigureAction_CallsConfiguredPaginationCriteria()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, int?>(p => p.Age, SortDirection.Ascending));

        var dbContext = unitOfWork.DbContext;
        var query = dbContext.Set<Person>().AsQueryable();

        // Act
        var subset = await query.ToPaginatedListAsync(paginator => paginator.SetPageNumber(1).SetPageSize(2).SortBy(p => p.Age));

        // Assert
        Assert.IsNotNull(subset);
        Assert.AreEqual(2, subset.Count);
        Assert.IsNull(subset[0].Age);
        Assert.AreEqual(12, subset[1].Age);
    }
}
