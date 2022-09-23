using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortingPaginatorTests
    {
        [TestMethod]
        public void PageSize_ByDefault_ReturnsDefaultPageSize()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            var pageSize = paginator.PageSize;

            // Assert
            Assert.AreEqual(SortingPaginator<Person>.DefaultPageSize, pageSize);
        }

        [TestMethod]
        public void PageNumber_ByDefault_ReturnsOne()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            var pageNumber = paginator.PageNumber;

            // Assert
            Assert.AreEqual(1, pageNumber);
        }

        [TestMethod]
        public void PageCount_ByDefault_ReturnsZero()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            var pageCount = paginator.PageCount;

            // Assert
            Assert.AreEqual(0, pageCount);
        }

        [TestMethod]
        public void ItemCount_ByDefault_ReturnsZero()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            var itemCount = paginator.ItemCount;

            // Assert
            Assert.AreEqual(0, itemCount);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void PageNumber_GivenInvalidValue_ThrowsArgumentOutOfRangeException(int pageNumber)
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.PageNumber = pageNumber;

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void PageSize_GivenInvalidValue_ThrowsArgumentOutOfRangeException(int pageSize)
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.PageSize = pageSize;

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void Paginate_WithNoSortProperties_ReturnsUnorderedPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(5);

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(5, result.Count());
            Assert.AreEqual("Welsh", result.First().FamilyName);
            Assert.AreEqual("Hensley", result.Last().FamilyName);
        }

        [TestMethod]
        public void Paginate_SortedByGivenNameAscending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(2);
            paginator.SortProperties.Add(new SortProperty<Person>(nameof(Person.GivenName), SortDirection.Ascending));

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Ana", result.First().GivenName);
            Assert.AreEqual("Dwayne", result.Last().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByFamilyNameDescending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(2);
            paginator.SortProperties.Add(new SortProperty<Person, string>(p => p.FamilyName!, SortDirection.Descending));

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Welsh", result.First().FamilyName);
            Assert.AreEqual("Stuart", result.Last().FamilyName);
        }

        [TestMethod]
        public void Paginate_SortedByFamilyNameAscendingThenGivenNameDescending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(2).WithPageSizeOf(3);
            paginator.SortedByProperty(nameof(Person.FamilyName));
            paginator.SortedByProperty(nameof(Person.GivenName), SortDirection.Descending);

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Phillipa", result.First().GivenName);
            Assert.AreEqual("Peter", result.ElementAt(1).GivenName);
            Assert.AreEqual("Glenn", result.Last().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByAgeAscending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(3);
            paginator.SortedByProperty(nameof(Person.Age), SortDirection.Ascending);

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Phillipa", result.First().GivenName); // null < any other value
            Assert.AreEqual("Peter", result.ElementAt(1).GivenName);
            Assert.AreEqual("Dwayne", result.Last().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByAgeDescending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(2);
            paginator.SortedByProperty(p => p.Age, SortDirection.Descending);

            // Act
            var result = paginator.Paginate(People.AsQueryable());

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Edgar", result.First().GivenName);
            Assert.AreEqual("Randall", result.Last().GivenName);
        }

        [TestMethod]
        public void SortedByProperty_GivenInvalidProperty_ThrowsArgumentException()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedByProperty("UndefinedProperty");

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Person'. (Parameter 'propertyName')", exception.Message);
        }

        [TestMethod]
        public void SortedByProperty_GivenInvalidNestedProperty_ThrowsArgumentException()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedByProperty("FavoriteBook.UndefinedProperty");

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Book'. (Parameter 'propertyName')", exception.Message);
        }
    }
}
