using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PaginationCriteriaTests
{
    [TestMethod]
    public void PageSize_ByDefault_ReturnsDefaultPageSize()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        var pageSize = paginationCriteria.PageSize;

        // Assert
        Assert.AreEqual(PaginationCriteria<Person>.DefaultPageSize, pageSize);
    }

    [TestMethod]
    public void PageNumber_ByDefault_ReturnsOne()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        var pageNumber = paginationCriteria.PageNumber;

        // Assert
        Assert.AreEqual(1, pageNumber);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public void PageNumber_GivenInvalidValue_ThrowsArgumentOutOfRangeException(int pageNumber)
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        void action() => paginationCriteria.PageNumber = pageNumber;

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(PaginationCriteria<Person>.PageNumber), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    [DataRow(int.MinValue)]
    public void PageSize_GivenInvalidValue_ThrowsArgumentOutOfRangeException(int pageSize)
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        void action() => paginationCriteria.PageSize = pageSize;

        // Assert
        var exception = Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        Assert.AreEqual(nameof(PaginationCriteria<Person>.PageSize), exception.ParamName);
    }

    [TestMethod]
    public void Apply_GivenNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        IQueryable<Person> query = null!;
        var paginationCriteria = new PaginationCriteria<Person>();

        // Act
        void action() => paginationCriteria.Apply(query);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(query), exception.ParamName);
    }

    [TestMethod]
    public void Apply_WithoutSortCriteria_ReturnsUnorderedPage()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 5);

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual("Welsh", result[0].FamilyName);
        Assert.AreEqual("Hensley", result[4].FamilyName);
    }

    [TestMethod]
    public void Apply_SortedByExpressionAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.GivenName, SortDirection.Ascending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Ana", result[0].GivenName);
        Assert.AreEqual("Dwayne", result[1].GivenName);
    }

    [TestMethod]
    public void Apply_SortedByExpressionDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.FamilyName, SortDirection.Descending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Welsh", result[0].FamilyName);
        Assert.AreEqual("Stuart", result[1].FamilyName);
    }

    [TestMethod]
    public void Apply_SortedByExpressionAscendingThenDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 2, pageSize: 3);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.FamilyName, SortDirection.Ascending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.GivenName, SortDirection.Descending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Phillipa", result[0].GivenName);
        Assert.AreEqual("Peter", result[1].GivenName);
        Assert.AreEqual("Glenn", result[2].GivenName);
    }

    [TestMethod]
    public void Apply_SortedByExpressionDescendingThenAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 3);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, int?>(p => p.Age, SortDirection.Descending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.GivenName, SortDirection.Ascending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Bernard", result[0].FamilyName);
        Assert.AreEqual("Bloom", result[1].FamilyName);
        Assert.AreEqual("Bryan", result[2].FamilyName);
    }

    [TestMethod]
    public void Apply_SortedByPropertyNameAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("GivenName", SortDirection.Ascending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Ana", result[0].GivenName);
        Assert.AreEqual("Dwayne", result[1].GivenName);
    }

    [TestMethod]
    public void Apply_SortedByPropertyNameDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 2);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("FamilyName", SortDirection.Descending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Welsh", result[0].FamilyName);
        Assert.AreEqual("Stuart", result[1].FamilyName);
    }

    [TestMethod]
    public void Apply_SortedByPropertyNameAscendingThenDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 2, pageSize: 3);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("FamilyName", SortDirection.Ascending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("GivenName", SortDirection.Descending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Phillipa", result[0].GivenName);
        Assert.AreEqual("Peter", result[1].GivenName);
        Assert.AreEqual("Glenn", result[2].GivenName);
    }

    [TestMethod]
    public void Apply_SortedByPropertyNameDescendingThenAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>(pageNumber: 1, pageSize: 3);
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("Age", SortDirection.Descending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("GivenName", SortDirection.Ascending));

        // Act
        var result = paginationCriteria.Apply(People.AsQueryable()).ToList();

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Bernard", result[0].FamilyName);
        Assert.AreEqual("Bloom", result[1].FamilyName);
        Assert.AreEqual("Bryan", result[2].FamilyName);
    }

    [TestMethod]
    public void ReadOnlySortCriteria_ByDefault_ReflectsSortCriteria()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("Age", SortDirection.Descending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person>("FamilyName", SortDirection.Ascending));
        paginationCriteria.SortCriteria.Add(new SortCriterion<Person, string>(p => p.GivenName, SortDirection.Ascending));

        // Act
        var readOnlyCriteria = ((IPaginationCriteria<Person>)paginationCriteria).SortCriteria;

        // Assert
        Assert.AreEqual(paginationCriteria.SortCriteria.Count, readOnlyCriteria.Count);
        for (var i = 0; i < readOnlyCriteria.Count; i++)
        {
            Assert.AreEqual(paginationCriteria.SortCriteria[i].Key, readOnlyCriteria[i].Key);
            Assert.AreEqual(paginationCriteria.SortCriteria[i].Direction, readOnlyCriteria[i].Direction);
        }
    }
}
