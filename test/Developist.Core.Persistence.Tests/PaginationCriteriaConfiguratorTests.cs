using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class PaginationCriteriaConfiguratorTests
{
    [TestMethod]
    public void Initialize_WithNullPaginationCriteria_ThrowsArgumentNullException()
    {
        // Arrange
        PaginationCriteria<Person> paginationCriteria = null!;

        // Act
        PaginationCriteriaConfigurator<Person> action() => new(paginationCriteria);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(paginationCriteria), exception.ParamName);
    }

    [TestMethod]
    public void StartAtPage_ForGivenValue_SetsPageNumber()
    {
        // Arrange
        const int pageNumber = 10;
        var paginationCriteria = new PaginationCriteria<Person>();
        var configurator = new PaginationCriteriaConfigurator<Person>(paginationCriteria);

        // Act
        configurator.StartAtPage(pageNumber);

        // Assert
        Assert.AreEqual(pageNumber, paginationCriteria.PageNumber);
    }

    [TestMethod]
    public void UsePageSize_ForGivenValue_SetsPageSize()
    {
        // Arrange
        const int pageSize = 30;
        var paginationCriteria = new PaginationCriteria<Person>();
        var configurator = new PaginationCriteriaConfigurator<Person>(paginationCriteria);

        // Act
        configurator.UsePageSize(pageSize);

        // Assert
        Assert.AreEqual(pageSize, paginationCriteria.PageSize);
    }

    [TestMethod]
    public void SortBy_GivenValidPropertySelector_AddsExpectedSortCriterion()
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();
        var configurator = new PaginationCriteriaConfigurator<Person>(paginationCriteria);

        // Act
        configurator.SortBy(p => p.Age, SortDirection.Descending);

        // Assert
        Assert.AreEqual(1, paginationCriteria.SortCriteria.Count);

        ISortCriterion sortCriterion = paginationCriteria.SortCriteria.Single();
        Assert.AreEqual(nameof(Person.Age), sortCriterion.Key);
        Assert.AreEqual(SortDirection.Descending, sortCriterion.Direction);
    }

    [TestMethod]
    public void SortBy_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortBy("UndefinedProperty");

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"No accessible property 'UndefinedProperty' defined on type '{nameof(Person)}'."));
    }

    [TestMethod]
    public void SortBy_GivenUndefinedNestedProperty_ThrowsArgumentException()
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortBy($"{nameof(Person.FavoriteBook)}.UndefinedProperty");

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"No accessible property 'UndefinedProperty' defined on type '{nameof(Book)}'."));
    }

    [TestMethod]
    public void SortByString_GivenNullString_ThrowsArgumentNullException()
    {
        // Arrange
        string sortString = null!;
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(sortString), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n")]
    public void SortByString_GivenEmptyOrWhiteSpaceString_ThrowsArgumentException(string sortString)
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(nameof(sortString), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow($"{nameof(Person.GivenName)} INVALID")]
    [DataRow($"{nameof(Person.GivenName)}, {nameof(Person.FamilyName)} ASCENDING")]
    [DataRow($"{nameof(Person.GivenName)}, {nameof(Person.FamilyName)} DESCENDING")]
    public void SortByString_GivenSortStringWithInvalidDirection_ThrowsFormatException(string sortString)
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<FormatException>(action);
        Assert.IsTrue(exception.Message.StartsWith("Invalid sort direction in sorting directive:"));
    }

    [DataTestMethod]
    [DataRow("UndefinedProperty")]
    [DataRow($"{nameof(Person.GivenName)}, UndefinedProperty")]
    public void SortByString_GivenSortStringWithUndefinedProperty_ThrowsFormatExceptionWithInnerException(string sortString)
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<FormatException>(action);
        Assert.AreEqual("Failed to parse a sorting directive from the input string. See the inner exception for details.", exception.Message);
        Assert.IsInstanceOfType<ArgumentException>(exception.InnerException);
    }

    [DataTestMethod]
    [DataRow("Undefined Property ASC")]
    [DataRow($"{nameof(Person.GivenName)}, Undefined Property DESC")]
    public void SortByString_GivenSortStringWithInvalidProperty_ThrowsFormatException(string sortString)
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<FormatException>(action);
        Assert.IsTrue(exception.Message.StartsWith("Invalid format for sorting directive:"));
    }

    [DataTestMethod]
    [DataRow($"{nameof(Person.GivenName)}, ")]
    [DataRow($"{nameof(Person.GivenName)}, , {nameof(Person.FamilyName)} DESC")]
    public void SortByString_GivenSortStringWithEmptyDirective_ThrowsFormatException(string sortString)
    {
        // Arrange
        var configurator = new PaginationCriteriaConfigurator<Person>(new PaginationCriteria<Person>());

        // Act
        void action() => configurator.SortByString(sortString);

        // Assert
        var exception = Assert.ThrowsException<FormatException>(action);
        Assert.AreEqual("Invalid sorting string format: The string contains an empty sorting directive.", exception.Message);
    }

    [DataTestMethod]
    [DataRow($"{nameof(Person.GivenName)}", new[] { nameof(Person.GivenName) }, new[] { SortDirection.Ascending })]
    [DataRow($"{nameof(Person.GivenName)}, {nameof(Person.FamilyName)}", new[] { nameof(Person.GivenName), nameof(Person.FamilyName) }, new[] { SortDirection.Ascending, SortDirection.Ascending })]
    [DataRow($"{nameof(Person.GivenName)} ASC, {nameof(Person.FamilyName)} asc", new[] { nameof(Person.GivenName), nameof(Person.FamilyName) }, new[] { SortDirection.Ascending, SortDirection.Ascending })]
    [DataRow($"{nameof(Person.GivenName)} DESC, {nameof(Person.FamilyName)} desc, {nameof(Person.Age)}", new[] { nameof(Person.GivenName), nameof(Person.FamilyName), nameof(Person.Age) }, new[] { SortDirection.Descending, SortDirection.Descending, SortDirection.Ascending })]
    [DataRow($"{nameof(Person.GivenName)}\tDESC", new[] { nameof(Person.GivenName) }, new[] { SortDirection.Descending })]
    [DataRow($"{nameof(Person.GivenName)}\r\nDESC", new[] { nameof(Person.GivenName) }, new[] { SortDirection.Descending })]
    [DataRow($"{nameof(Person.GivenName)} \t\r\nDESC", new[] { nameof(Person.GivenName) }, new[] { SortDirection.Descending })]
    public void SortByString_GivenValidSortString_AddsExpectedSortCriteria(
        string sortString,
        string[] expectedPropertyNames,
        SortDirection[] expectedDirections)
    {
        // Arrange
        var paginationCriteria = new PaginationCriteria<Person>();
        var configurator = new PaginationCriteriaConfigurator<Person>(paginationCriteria);

        // Act
        configurator.SortByString(sortString);

        // Assert
        var actualProperties = paginationCriteria.SortCriteria.Cast<ISortCriterion>();

        var actualPropertyNames = actualProperties.Select(prop => prop.Key).ToArray();
        var actualDirections = actualProperties.Select(prop => prop.Direction).ToArray();

        Assert.IsTrue(expectedPropertyNames.SequenceEqual(actualPropertyNames));
        Assert.IsTrue(expectedDirections.SequenceEqual(actualDirections));
    }
}
