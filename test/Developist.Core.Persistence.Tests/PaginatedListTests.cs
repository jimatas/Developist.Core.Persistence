using Developist.Core.Persistence.Tests.Fixture;
using System.Collections;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PaginatedListTests
{
    [TestMethod]
    public void Empty_ByDefault_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var paginatedList = PaginatedList<Person>.Empty;

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.IsTrue(paginatedList.Count == 0);
        Assert.AreEqual(0, paginatedList.PageCount);
        Assert.AreEqual(1, paginatedList.PageNumber);
        Assert.AreEqual(1, paginatedList.PageSize);
    }

    [TestMethod]
    public void GetEnumerator_ThroughEnumerableReference_ReturnsEnumerator()
    {
        // Arrange
        var people = People.AsQueryable();

        IEnumerable enumerable = new PaginatedList<Person>(
            people.Take(5).ToList(),
            pageNumber: 1,
            pageSize: 5,
            itemCount: people.Count());

        // Act
        var enumerator = enumerable.GetEnumerator();

        // Assert
        Assert.IsNotNull(enumerator);
        Assert.IsTrue(enumerator.MoveNext());
    }

    [TestMethod]
    public void Initialize_ByDefault_SetsAllProperties()
    {
        // Arrange
        const int pageSize = 5;
        var people = People.AsQueryable();
        var subset = people.Take(pageSize).ToList();

        // Act
        var paginatedList = new PaginatedList<Person>(subset,
            pageNumber: 1,
            pageSize,
            itemCount: people.Count());

        // Assert
        Assert.AreEqual(1, paginatedList.PageNumber);
        Assert.AreEqual(pageSize, paginatedList.PageSize);
        Assert.AreEqual(people.Count(), paginatedList.ItemCount);
        Assert.AreEqual((int)Math.Ceiling((double)people.Count() / pageSize), paginatedList.PageCount);
        Assert.AreEqual(pageSize, paginatedList.Count);
    }

    [TestMethod]
    public void Initialize_GivenNullInnerList_ThrowsArgumentNullException()
    {
        // Arrange
        IReadOnlyList<Person>? innerList = null;

        // Act
        var action = () => new PaginatedList<Person>(innerList!,
            pageNumber: 1,
            pageSize: 5,
            itemCount: 10);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(innerList), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Initialize_GivenInvalidPageNumber_ThrowsArgumentOutOfRangeException(int pageNumber)
    {
        // Arrange
        const int pageSize = 5;
        var people = People.AsQueryable();

        // Act
        var action = () => new PaginatedList<Person>(people.Take(pageSize).ToList(),
            pageNumber,
            pageSize,
            itemCount: people.Count());

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(pageNumber), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Initialize_GivenInvalidPageSize_ThrowsArgumentOutOfRangeException(int pageSize)
    {
        // Arrange
        var people = People.AsQueryable();

        // Act
        var action = () => new PaginatedList<Person>(people.Take(5).ToList(),
            pageNumber: 1,
            pageSize,
            itemCount: people.Count());

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(pageSize), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(1, -1, 5)]
    [DataRow(1, 0, 5)]
    [DataRow(1, 4, 5)]
    [DataRow(2, 3, 4)]
    public void Initialize_GivenInvalidItemCount_ThrowsArgumentOutOfRangeException(int pageNumber, int itemCount, int reportedMinCount)
    {
        // Arrange
        const int pageSize = 5;

        // Act
        var action = () => new PaginatedList<Person>(People.AsQueryable().Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            pageNumber,
            pageSize,
            itemCount);

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(itemCount), exception.ParamName);
        Assert.IsTrue(exception.Message.StartsWith($"Value cannot be less than {reportedMinCount}."));
    }
}
