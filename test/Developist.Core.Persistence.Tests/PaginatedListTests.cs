using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PaginatedListTests
{
    [TestMethod]
    public void Empty_Always_ReturnsEmptyList()
    {
        // Arrange

        // Act
        var paginatedList = PaginatedList<Person>.Empty;

        // Assert
        Assert.IsNotNull(paginatedList);
        Assert.AreEqual(0, paginatedList.Count);
        Assert.AreEqual(0, paginatedList.PageCount);
        Assert.AreEqual(1, paginatedList.PageNumber);
        Assert.AreEqual(PaginationCriteria<Person>.DefaultPageSize, paginatedList.PageSize);
    }

    [TestMethod]
    public void GetEnumerator_ViaEnumerableReference_ReturnsEnumerator()
    {
        // Arrange
        var people = People.AsEnumerable();
        IEnumerable enumerable = new PaginatedList<Person>(
            pageNumber: 1,
            pageSize: 5,
            innerList: people.Take(5).ToList(),
            totalCount: people.Count());

        // Act
        var enumerator = enumerable.GetEnumerator();

        // Assert
        Assert.IsNotNull(enumerator);
        Assert.IsTrue(enumerator.MoveNext());
    }

    [TestMethod]
    public void Initialize_WithDefaultValues_SetsAllProperties()
    {
        // Arrange
        const int pageSize = 5;
        var people = People.AsEnumerable();
        var subset = people.Take(pageSize).ToList();

        // Act
        var paginatedList = new PaginatedList<Person>(
            pageNumber: 1,
            pageSize,
            innerList: subset,
            totalCount: people.Count());

        // Assert
        Assert.AreEqual(1, paginatedList.PageNumber);
        Assert.AreEqual(pageSize, paginatedList.PageSize);
        Assert.AreEqual(people.Count(), paginatedList.TotalCount);
        Assert.AreEqual((int)Math.Ceiling((double)people.Count() / pageSize), paginatedList.PageCount);
        Assert.AreEqual(pageSize, paginatedList.Count);
    }

    [TestMethod]
    public void Initialize_WithNullInnerList_ThrowsArgumentNullException()
    {
        // Arrange
        IReadOnlyList<Person> innerList = null!;

        // Act
        void action() => _ = new PaginatedList<Person>(
            pageNumber: 1,
            pageSize: 5,
            innerList,
            totalCount: 10);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(innerList), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Initialize_WithInvalidPageNumber_ThrowsArgumentOutOfRangeException(int pageNumber)
    {
        // Arrange
        const int pageSize = 5;
        var people = People.AsEnumerable();

        // Act
        void action() => _ = new PaginatedList<Person>(
            pageNumber,
            pageSize,
            innerList: people.Take(pageSize).ToList(),
            totalCount: people.Count());

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(pageNumber), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(-1)]
    [DataRow(0)]
    public void Initialize_WithInvalidPageSize_ThrowsArgumentOutOfRangeException(int pageSize)
    {
        // Arrange
        var people = People.AsEnumerable();

        // Act
        void action() => _ = new PaginatedList<Person>(
            pageNumber: 1,
            pageSize,
            innerList: people.Take(5).ToList(),
            totalCount: people.Count());

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(pageSize), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(1, -1, 5)]
    [DataRow(1, 0, 5)]
    [DataRow(1, 4, 5)]
    [DataRow(2, 3, 4)]
    public void Initialize_WithInvalidTotalCount_ThrowsArgumentOutOfRangeException(int pageNumber, int totalCount, int reportedMinCount)
    {
        // Arrange
        const int pageSize = 5;
        var people = People.AsEnumerable();

        // Act
        void action() => _ = new PaginatedList<Person>(
            pageNumber,
            pageSize,
            innerList: people.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
            totalCount);

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(totalCount), exception.ParamName);
        Assert.IsTrue(exception.Message.StartsWith($"Value must be greater than or equal to {reportedMinCount}."));
    }

    [DataTestMethod]
    [DataRow(1, 1)]
    [DataRow(1, -1)]
    public void Indexer_GivenOutOrRangeIndex_ThrowsArgumentOutOfRangeException(int pageSize, int index)
    {
        // Arrange
        var people = People.AsEnumerable();
        var paginatedList = new PaginatedList<Person>(
            pageNumber: 1,
            pageSize,
            innerList: people.Take(pageSize).ToList(),
            totalCount: people.Count());

        // Act
        void action() => _ = paginatedList[index];

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(index), exception.ParamName);
    }
}
