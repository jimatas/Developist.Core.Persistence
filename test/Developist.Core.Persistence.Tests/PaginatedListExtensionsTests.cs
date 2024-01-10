using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PaginatedListExtensionsTests
{
    [TestMethod]
    public void HasNextPage_WhenNextPageIsAvailable_ReturnsTrue()
    {
        // Arrange
        var people = People.AsEnumerable();
        var paginatedList = new PaginatedList<Person>(
            innerList: people.Take(5).ToList(),
            pageNumber: 1,
            pageSize: 5,
            totalCount: people.Count());

        // Act
        var result = paginatedList.HasNextPage();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasNextPage_WhenNextPageIsUnavailable_ReturnsFalse()
    {
        // Arrange
        var people = People.AsEnumerable();
        var paginatedList = new PaginatedList<Person>(
            innerList: people.Take(5).ToList(),
            pageNumber: 2,
            pageSize: 5,
            totalCount: people.Count());

        // Act
        var result = paginatedList.HasNextPage();

        // Assert
        Assert.IsFalse(result);
    }

    [TestMethod]
    public void HasPreviousPage_WhenPreviousPageIsAvailable_ReturnsTrue()
    {
        // Arrange
        var people = People.AsEnumerable();
        var paginatedList = new PaginatedList<Person>(
            innerList: people.Take(5).ToList(),
            pageNumber: 2,
            pageSize: 5,
            totalCount: people.Count());

        // Act
        var result = paginatedList.HasPreviousPage();

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public void HasPreviousPage_WhenPreviousPageIsUnavailable_ReturnsFalse()
    {
        // Arrange
        var people = People.AsEnumerable();
        var paginatedList = new PaginatedList<Person>(
            innerList: people.Take(5).ToList(),
            pageNumber: 1,
            pageSize: 5,
            totalCount: people.Count());

        // Act
        var result = paginatedList.HasPreviousPage();

        // Assert
        Assert.IsFalse(result);
    }
}
