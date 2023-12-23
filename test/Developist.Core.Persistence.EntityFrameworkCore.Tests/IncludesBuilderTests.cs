using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class IncludesBuilderTests
{
    [TestMethod]
    public void NewInstance_ByDefault_HasNoPaths()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act

        // Assert
        Assert.IsFalse(includes.AsList().Any());
    }

    [TestMethod]
    public void Include_GivenNullString_ThrowsArgumentNullException()
    {
        // Arrange
        string path = null!;
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        void action() => includes.Include(path);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(path), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n\t")]
    public void Include_GivenEmptyOrWhiteSpaceString_ThrowsArgumentException(string path)
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        void action() => includes.Include(path);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(nameof(path), exception.ParamName);
    }

    [TestMethod]
    public void Include_GivenNonExistentPath_IncludesPath()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes.Include("UndefinedProperty");

        // Assert
        Assert.IsTrue(includes.AsList().Contains("UndefinedProperty"));
    }

    [TestMethod]
    public void Include_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        void action() => includes.Include(p => (object)"UndefinedProperty");

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void Include_GivenValidPath_IncludesPath()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes.Include(nameof(Person.FamilyName));

        // Assert
        Assert.IsTrue(includes.AsList().Contains(nameof(Person.FamilyName)));
    }

    [TestMethod]
    public void Include_GivenValidProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes = includes.Include(person => person.FamilyName);

        // Assert
        Assert.IsTrue(includes.AsList().Contains(nameof(Person.FamilyName)));
    }

    [TestMethod]
    public void Include_GivenValidCollectionProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludesBuilder<SocialPerson> includes = new IncludesBuilder<SocialPerson>();

        // Act
        includes = includes.Include(person => person.Friends);

        // Assert
        Assert.IsTrue(includes.AsList().Contains(nameof(SocialPerson.Friends)));
    }

    [TestMethod]
    public void Include_GivenSamePathTwice_IncludesBothPaths()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes.Include(nameof(Person.FamilyName));
        includes.Include(nameof(Person.FamilyName));

        // Assert
        Assert.AreEqual(2, includes.AsList().Count);
    }

    [TestMethod]
    public void Include_GivenSamePathAsStringAndExpression_IncludesBothPaths()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes.Include(nameof(Person.FamilyName));
        includes.Include(p => p.FamilyName);

        // Assert
        Assert.AreEqual(2, includes.AsList().Count);
    }

    [TestMethod]
    public void Include_GivenTwoDifferentPaths_IncludesBothPaths()
    {
        // Arrange
        IIncludesBuilder<Person> includes = new IncludesBuilder<Person>();

        // Act
        includes.Include(nameof(Person.GivenName));
        includes.Include(nameof(Person.FamilyName));

        // Assert
        Assert.AreEqual(2, includes.AsList().Count);
    }

    [TestMethod]
    public void ThenInclude_WithoutUsingIncludeFirst_ThrowsInvalidOperationException()
    {
        // Arrange
        var includes = new IncludesBuilder<Person, Book>(new List<string>());

        // Act
        void action() => includes.ThenInclude(book => book.Authors);

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual($"The 'ThenInclude' method requires an initial path to be set before it can be used.", exception.Message);
    }

    [TestMethod]
    public void ThenInclude_GivenNullExpression_ThrowsArgumentException()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);
        Expression<Func<Book?, object>> propertySelector = null!;
        var pathSegment = string.Empty;

        // Act
        void action() => includes.ThenInclude(propertySelector);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(nameof(pathSegment), exception.ParamName);
    }

    [TestMethod]
    public void ThenInclude_GivenNullString_ThrowsArgumentNullException()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);
        string pathSegment = null!;

        // Act
        void action() => includes.ThenInclude(pathSegment);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(pathSegment), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n\t")]
    public void ThenInclude_GivenEmptyOrWhiteSpaceString_ThrowsArgumentException(string pathSegment)
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        void action() => includes.ThenInclude(pathSegment);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(nameof(pathSegment), exception.ParamName);
    }

    [TestMethod]
    public void ThenInclude_GivenNonExistentPath_IncludesPath()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includes.ThenInclude("UndefinedProperty");

        // Assert
        Assert.IsTrue(includes.AsList().Contains($"{nameof(Person.FavoriteBook)}.UndefinedProperty"));
    }

    [TestMethod]
    public void ThenInclude_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        void action() => includes.ThenInclude(p => (object)"UndefinedProperty");

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void ThenInclude_GivenValidPath_IncludesPath()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includes.ThenInclude(nameof(Book.Authors));

        // Assert
        Assert.IsTrue(includes.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
    }

    [TestMethod]
    public void ThenInclude_GivenValidProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includes.ThenInclude(book => book!.Title);

        // Assert
        Assert.IsTrue(includes.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Title)}"));
    }

    [TestMethod]
    public void ThenInclude_GivenValidCollectionProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludesBuilder<Person, Book?> includes = new IncludesBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includes.ThenInclude(book => book!.Authors);

        // Assert
        Assert.IsTrue(includes.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
    }

    [TestMethod]
    public void ThenInclude_WithPreviousProperty_IncludesPath()
    {
        // Arrange
        IIncludesBuilder<SocialPerson> includes = new IncludesBuilder<SocialPerson>();

        // Act
        includes
            .Include(p => p.Friends).ThenInclude(f => f.FavoriteBook)
            .Include(p => p.Friends).ThenInclude(f => f.Friends);

        // Assert
        var paths = includes.AsList();
        Assert.AreEqual(2, paths.Count);
        Assert.IsTrue(paths.Contains($"{nameof(SocialPerson.Friends)}.{nameof(Person.FavoriteBook)}"));
        Assert.IsTrue(paths.Contains($"{nameof(SocialPerson.Friends)}.{nameof(SocialPerson.Friends)}"));
    }
}
