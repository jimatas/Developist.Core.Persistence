using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortPropertyTests
    {
        [TestMethod]
        public void Initialize_GivenNullPropertyName_ThrowsArgumentNullException()
        {
            // Arrange
            string? propertyName = null;

            // Act
            void action() => _ = new SortProperty<Person>(propertyName!, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" \r\n\t")]
        public void Initialize_GivenEmptyOrWhiteSpacePropertyName_ThrowsArgumentException(string propertyName)
        {
            // Arrange

            // Act
            void action() => _ = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Initialize_GivenNullPropertyExpression_ThrowsArgumentNullException()
        {
            // Arrange
            Expression<Func<Person, object>>? propertySelector = null;

            // Act
            void action() => _ = new SortProperty<Person, object>(propertySelector!, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Initialize_GivenInvalidPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
        {
            // Arrange
            var propertyName = "UndefinedProperty";

            // Act
            void action() => _ = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Person'. (Parameter 'propertyName')", exception.Message);
        }

        [TestMethod]
        public void Initialize_GivenInvalidNestedPropertyName_ThrowsArgumentExceptionWithExpectedMessage()
        {
            // Arrange
            var propertyName = "FavoriteBook.UndefinedProperty";

            // Act
            void action() => _ = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Book'. (Parameter 'propertyName')", exception.Message);
        }

        [TestMethod]
        public void Initialize_GivenValidPropertyName_SetsPropertyExpression()
        {
            // Arrange
            var propertyName = "FamilyName";

            // Act
            var property = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.IsNotNull(property.Property);
        }
    }
}
