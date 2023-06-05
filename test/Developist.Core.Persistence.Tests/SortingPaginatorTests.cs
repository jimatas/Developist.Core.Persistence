using Developist.Core.Persistence.Tests.Fixture;
using Developist.Core.Persistence.Tests.Helpers;

namespace Developist.Core.Persistence.Tests;

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
        Action action = () => paginator.PageNumber = pageNumber;

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
        Action action = () => paginator.PageSize = pageSize;

        // Assert
        Assert.ThrowsException<ArgumentOutOfRangeException>(action);
    }

    [TestMethod]
    public void PaginateAsync_GivenNullQuery_ThrowsArgumentNullException()
    {
        // Arrange
        IQueryable<Person>? query = null;
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => paginator.PaginateAsync(query!);

        // Assert
        Assert.ThrowsException<ArgumentNullException>(action);
    }

    [TestMethod]
    public async Task PaginateAsync_WithNoSortProperties_ReturnsUnorderedPage()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(5);

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(5, result.Count);
        Assert.AreEqual("Dwayne Welsh", result[0].FullName);
        Assert.AreEqual("Glenn Hensley", result[4].FullName);
    }

    [TestMethod]
    public async Task PaginateAsync_SortedByGivenNameAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(2);
        paginator.SortableProperties.Add(new SortableProperty<Person>(nameof(Person.GivenName), SortDirection.Ascending));

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Ana Bryan", result[0].FullName);
        Assert.AreEqual("Dwayne Welsh", result[1].FullName);
    }

    [TestMethod]
    public async Task PaginateAsync_SortedByFamilyNameDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(2);
        paginator.SortableProperties.Add(new SortableProperty<Person, string>(p => p.FamilyName!, SortDirection.Descending));

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Dwayne Welsh", result[0].FullName);
        Assert.AreEqual("Ed Stuart", result[1].FullName);
    }

    [TestMethod]
    public async Task PaginateAsync_SortedByFamilyNameAscendingThenGivenNameDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(2).WithPageSize(3);
        paginator.SortedByProperty(nameof(Person.FamilyName));
        paginator.SortedByProperty(nameof(Person.GivenName), SortDirection.Descending);

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Phillipa Connor", result[0].FullName);
        Assert.AreEqual("Peter Connor", result[1].FullName);
        Assert.AreEqual("Glenn Hensley", result[2].FullName);
    }

    [TestMethod]
    public async Task PaginateAsync_SortedByAgeAscending_ReturnsExpectedResult()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(3);
        paginator.SortedByProperty(nameof(Person.Age), SortDirection.Ascending);

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(3, result.Count);
        Assert.AreEqual("Phillipa Connor", result[0].FullName); // null < any other value.
        Assert.AreEqual("Peter Connor", result[1].FullName);
        Assert.AreEqual("Dwayne Welsh", result[2].FullName);
    }

    [TestMethod]
    public async Task PaginateAsync_SortedByAgeDescending_ReturnsExpectedResult()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSize(2);
        paginator.SortedByProperty(p => p.Age, SortDirection.Descending);

        // Act
        var result = await paginator.PaginateAsync(People.AsQueryable());

        // Assert
        Assert.AreEqual(2, result.Count);
        Assert.AreEqual("Edgar Bernard", result[0].FullName);
        Assert.AreEqual("Randall Bloom", result[1].FullName);
    }

    [TestMethod]
    public void SortedByProperty_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => paginator.SortedByProperty("UndefinedProperty");

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("No accessible property 'UndefinedProperty' defined on type 'Person'. (Parameter 'propertyName')", exception.Message);
    }

    [TestMethod]
    public void SortedByProperty_GivenUndefinedNestedProperty_ThrowsArgumentException()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => paginator.SortedByProperty("FavoriteBook.UndefinedProperty");

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual("No accessible property 'UndefinedProperty' defined on type 'Book'. (Parameter 'propertyName')", exception.Message);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n")]
    public void SortedByString_GivenNullEmptyOrWhiteSpaceString_ThrowsArgumentException(string value)
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => paginator.SortedByString(value);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [DataTestMethod]
    [DataRow("+()")]
    [DataRow("GivenName,-( )")]
    public void SortedByString_GivenInvalidDirectives_ThrowsFormatException(string value)
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        var action = () => paginator.SortedByString(value);

        // Assert
        Assert.ThrowsException<FormatException>(action);
    }

    [TestMethod]
    public void SortedByString_GivenSingleDirective_AddsSortProperty()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        paginator.SortedByString("FamilyName");

        // Assert
        Assert.AreEqual(1, paginator.SortableProperties.Count);

        var sortableProperty = (SortableProperty<Person>)paginator.SortableProperties.First();
        Assert.AreEqual(nameof(Person.FamilyName), sortableProperty.Property.GetMemberName());
        Assert.AreEqual(SortDirection.Ascending, sortableProperty.Direction);
    }

    [TestMethod]
    public void SortedByString_GivenTwoDirectives_AddsBoth()
    {
        // Arrange
        var paginator = new SortingPaginator<Person>();

        // Act
        paginator.SortedByString("FamilyName,-Age");

        // Assert
        Assert.AreEqual(2, paginator.SortableProperties.Count);

        var sortableProperty = (SortableProperty<Person>)paginator.SortableProperties.First();
        Assert.AreEqual(nameof(Person.FamilyName), sortableProperty.Property.GetMemberName());
        Assert.AreEqual(SortDirection.Ascending, sortableProperty.Direction);

        sortableProperty = (SortableProperty<Person>)paginator.SortableProperties.Last();
        Assert.AreEqual(nameof(Person.Age), sortableProperty.Property.GetMemberName());
        Assert.AreEqual(SortDirection.Descending, sortableProperty.Direction);
    }
}
