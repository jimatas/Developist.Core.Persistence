using Developist.Core.Persistence.Pagination.Sorting;
using Developist.Core.Persistence.Tests.Fixture;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class SortablePropertyTests
{
    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n")]
    public void Initialize_GivenNullEmptyOrWhiteSpacePropertyName_ThrowsArgumentException(string propertyName)
    {
        // Arrange

        // Act
        var action = () => new SortableProperty<Person>(propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("propertyName", exception.ParamName);
    }

    [TestMethod]
    public void Initialize_GivenNullPropertyExpression_ThrowsArgumentNullException()
    {
        // Arrange
        Expression<Func<Person, object>>? propertySelector = null;

        // Act
        var action = () => new SortableProperty<Person, object>(propertySelector!, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual("property", exception.ParamName);
    }

    [TestMethod]
    public void Initialize_GivenUndefinedPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        var propertyName = "UndefinedProperty";

        // Act
        var action = () => new SortableProperty<Person>(propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("No accessible property 'UndefinedProperty' defined on type 'Person'. (Parameter 'propertyName')", exception.Message);
    }

    [TestMethod]
    public void Initialize_GivenUndefinedNestedPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
    {
        // Arrange
        var propertyName = "FavoriteBook.UndefinedProperty";

        // Act
        var action = () => new SortableProperty<Person>(propertyName, SortDirection.Ascending);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("No accessible property 'UndefinedProperty' defined on type 'Book'. (Parameter 'propertyName')", exception.Message);
    }

    [TestMethod]
    public void Initialize_GivenDefinedPropertyName_SetsPropertyExpression()
    {
        // Arrange
        var propertyName = "FamilyName";

        // Act
        var sortableProperty = new SortableProperty<Person>(propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortableProperty.Property);
    }

    [TestMethod]
    public void Initialize_GivenNestedPropertyName_SetsPropertyExpression()
    {
        // Arrange
        var propertyName = "FavoriteBook.Genre";

        // Act
        var sortableProperty = new SortableProperty<Person>(propertyName, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortableProperty.Property);
    }

    [TestMethod]
    public void Initialize_GivenPropertySelector_SetsPropertyExpression()
    {
        // Arrange
        Expression<Func<Person, string>> propertySelector = p => p.FamilyName;

        // Act
        var sortableProperty = new SortableProperty<Person, string>(propertySelector, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortableProperty.Property);
    }

    [TestMethod]
    public void Initialize_GivenNestedPropertySelector_SetsPropertyExpression()
    {
        // Arrange
        Expression<Func<Person, Genre>> propertySelector = p => p.FavoriteBook!.Genre;

        // Act
        var sortableProperty = new SortableProperty<Person, Genre>(propertySelector, SortDirection.Ascending);

        // Assert
        Assert.IsNotNull(sortableProperty.Property);
    }
}
