using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Pagination.Sorting;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class QueryableExtensionsTests
{
    [TestMethod]
    public async Task ToPaginatedListAsync_GivenNullPaginator_ThrowsArgumentNullException()
    {
        // Arrange
        SortingPaginator<Person>? paginator = null;

        // Act
        var action = () => People.AsQueryable().ToPaginatedListAsync(paginator!);

        // Assert
        var exception = await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        Assert.AreEqual(nameof(paginator), exception.ParamName);
    }

    [TestMethod]
    public async Task ToPaginatedListAsync_GivenValidPaginator_CallsIt()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2).SortedByProperty(p => p.Age, SortDirection.Descending);

        // Act
        var subset = await People.AsQueryable().ToPaginatedListAsync(paginator);

        // Assert
        Assert.IsNotNull(subset);
        Assert.AreEqual(2, subset.Count);
        Assert.AreEqual(80, subset[0].Age);
        Assert.AreEqual(55, subset[1].Age);
    }

    [TestMethod]
    public void SortBy_GivenNullProperty_ThrowsArgumentNullException()
    {
        // Arrange
        SortablePropertyBase<Person>? property = null;

        // Act
        var action = () => People.AsQueryable().SortBy(property);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(property), exception.ParamName);
    }

    [TestMethod]
    public void SortBy_GivenValidProperty_CallsIt()
    {
        // Arrange
        var unsorted = People.AsQueryable();

        // Act
        var sorted = unsorted.SortBy(new SortableProperty<Person>(nameof(Person.FamilyName), SortDirection.Ascending));

        // Assert
        Assert.IsFalse(unsorted.SequenceEqual(sorted));
    }

    [TestMethod]
    public void FilterBy_GivenNullCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        IFilterCriteria<Person>? criteria = null;

        // Act
        var action = () => People.AsQueryable().FilterBy(criteria);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(criteria), exception.ParamName);
    }

    [TestMethod]
    public void FilterBy_GivenValidCriteria_CallsIt()
    {
        // Arrange
        var unfiltered = People.AsQueryable();

        // Act
        var filtered = unfiltered.FilterBy(new PersonByNameCriteria { FamilyName = "Connor" });

        // Assert
        Assert.AreEqual(2, filtered.Count());
        Assert.AreEqual("Connor", filtered.First().FamilyName);
        Assert.AreEqual("Connor", filtered.Last().FamilyName);
    }
}
