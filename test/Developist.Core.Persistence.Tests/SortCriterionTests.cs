using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class SortCriterionTests
{
    [TestMethod]
    public void Initialize_WithDefaultValues_SetsKeyAndDirection()
    {
        // Arrange
        var propertyName = nameof(Person.FamilyName);
        var direction = SortDirection.Descending;

        // Act
        ISortCriterion sortCriterion = new SortCriterion<Person>(key: propertyName, direction);

        // Assert
        Assert.AreEqual(propertyName, sortCriterion.Key);
        Assert.AreEqual(direction, sortCriterion.Direction);
    }

    [TestMethod]
    public void Initialize_WithNullPropertyName_ThrowsArgumentNullException()
    {
        // Arrange
        string propertyName = null!;

        // Act
        SortCriterion<Person> action() => new(key: propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual("key", exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n")]
    public void Initialize_WithEmptyOrWhiteSpacePropertyName_ThrowsArgumentException(string propertyName)
    {
        // Arrange

        // Act
        SortCriterion<Person> action() => new(key: propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("key", exception.ParamName);
    }

    [DataTestMethod]
    [DataRow(SortDirection.Ascending - 1)]
    [DataRow(SortDirection.Descending + 1)]
    public void Initialize_WithInvalidSortDirection_ThrowsInvalidEnumArgumentException(SortDirection direction)
    {
        // Arrange

        // Act
        SortCriterion<Person> action() => new(nameof(Person.FamilyName), direction);

        // Assert
        var exception = Assert.ThrowsException<InvalidEnumArgumentException>(action);
        Assert.AreEqual(nameof(direction), exception.ParamName);
    }

    [TestMethod]
    public void Initialize_WithNullPropertySelector_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<Person, object>> propertySelector = null!;

        // Act
        SortCriterion<Person, object> action() => new(key: propertySelector, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual("key", exception.ParamName);
    }

    [TestMethod]
    public void Initialize_WithUndefinedPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        var propertyName = "UndefinedProperty";

        // Act
        SortCriterion<Person> action() => new(key: propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"No accessible property 'UndefinedProperty' defined on type '{nameof(Person)}'."));
    }

    [TestMethod]
    public void Initialize_WithUndefinedNestedPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        var propertyName = $"{nameof(Person.FavoriteBook)}.UndefinedProperty";

        // Act
        SortCriterion<Person> action() => new(key: propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith($"No accessible property 'UndefinedProperty' defined on type '{nameof(Book)}'."));
    }

    [TestMethod]
    public void Initialize_WithInvalidPropertySelector_ThrowsArgumentException()
    {
        // Arrange
        Expression<Func<Person, string>> propertySelector = p => p.FavoriteBook!.Title ?? string.Empty;

        // Act
        SortCriterion<Person, string> action() => new(key: propertySelector, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.IsTrue(exception.Message.StartsWith("The provided lambda expression must represent a property."));
    }

    [TestMethod]
    public void Initialize_WithValidPropertyName_SetsKey()
    {
        // Arrange
        var propertyName = nameof(Person.FamilyName);

        // Act
        var sortCriterion = new SortCriterion<Person>(key: propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
        Assert.AreEqual(propertyName, ((ISortCriterion)sortCriterion).Key);
    }

    [TestMethod]
    public void Initialize_WithValidPropertyNameWrongCase_SetsKey()
    {
        // Arrange
        var propertyName = nameof(Person.FamilyName).ToLowerInvariant();

        // Act
        var sortCriterion = new SortCriterion<Person>(key: propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
        Assert.AreEqual(propertyName, ((ISortCriterion)sortCriterion).Key);
    }

    [TestMethod]
    public void Initialize_WithValidNestedPropertyName_SetsKey()
    {
        // Arrange
        var propertyName = $"{nameof(Person.FavoriteBook)}.{nameof(Book.Title)}";

        // Act
        var sortCriterion = new SortCriterion<Person>(key: propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
        Assert.AreEqual(propertyName, ((ISortCriterion)sortCriterion).Key);
    }

    [TestMethod]
    public void Initialize_WithValidNestedPropertyNameWrongCase_SetsKey()
    {
        // Arrange
        var propertyName = $"{nameof(Person.FavoriteBook)}.{nameof(Book.Title)}".ToUpperInvariant();

        // Act
        var sortCriterion = new SortCriterion<Person>(key: propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
        Assert.AreEqual(propertyName, ((ISortCriterion)sortCriterion).Key);
    }

    [TestMethod]
    public void Initialize_WithValidPropertySelector_SetsKey()
    {
        // Arrange
        Expression<Func<Person, string>> propertySelector = p => p.FamilyName!;

        // Act
        var sortCriterion = new SortCriterion<Person, string>(key: propertySelector, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
    }

    [TestMethod]
    public void Initialize_WithValidNestedPropertySelector_SetsKey()
    {
        // Arrange
        Expression<Func<Person, string>> propertySelector = p => p.FavoriteBook!.Title;

        // Act
        var sortCriterion = new SortCriterion<Person, string>(key: propertySelector, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
    }

    [TestMethod]
    public void Initialize_WithUnaryExpressionInPropertySelector_SetsKey()
    {
        // Arrange
        Expression<Func<Person, string>> propertySelector = p => (string)p.FavoriteBook!.Title; // Using a redundant cast to produce a unary expression.

        // Act
        var sortCriterion = new SortCriterion<Person, string>(key: propertySelector, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortCriterion.Key);
        Assert.IsNotNull(((ISortCriterion)sortCriterion).Key);
    }
}
