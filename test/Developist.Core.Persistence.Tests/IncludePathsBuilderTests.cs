﻿using Developist.Core.Persistence.IncludePaths;
using Developist.Core.Persistence.Tests.Fixture;
using System.Reflection;

namespace Developist.Core.Persistence.Tests;

[TestClass]
public class IncludePathsBuilderTests
{
    [TestMethod]
    public void NewInstance_ByDefault_HasNoPaths()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act

        // Assert
        Assert.IsFalse(includePaths.AsList().Any());
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n\t")]
    public void Include_GivenNullOrEmptyOrWhiteSpaceString_ThrowsArgumentException(string path)
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        void action() => includePaths.Include(path);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void Include_GivenNonExistentPath_IncludesIt()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths.Include("UndefinedProperty");

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains("UndefinedProperty"));
    }

    [TestMethod]
    public void Include_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        void action() => includePaths.Include(p => (object)"UndefinedProperty");

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void Include_GivenValidPath_IncludesIt()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths.Include(nameof(Person.FamilyName));

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains(nameof(Person.FamilyName)));
    }

    [TestMethod]
    public void Include_GivenValidProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths = includePaths.Include(person => person.FamilyName);

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains(nameof(Person.FamilyName)));
    }

    [TestMethod]
    public void Include_GivenValidCollectionProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths = includePaths.Include(person => person.Friends);

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains(nameof(Person.Friends)));
    }

    [TestMethod]
    public void Include_GivenSamePathTwice_IncludesBoth()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths.Include(nameof(Person.FamilyName));
        includePaths.Include(nameof(Person.FamilyName));

        // Assert
        Assert.AreEqual(2, includePaths.AsList().Count);
    }

    [TestMethod]
    public void Include_GivenSamePathAsStringAndExpression_IncludesBoth()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths.Include(nameof(Person.FamilyName));
        includePaths.Include(p => p.FamilyName);

        // Assert
        Assert.AreEqual(2, includePaths.AsList().Count);
    }

    [TestMethod]
    public void Include_GivenTwoDifferentPaths_IncludesBoth()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths.Include(nameof(Person.GivenName));
        includePaths.Include(nameof(Person.FamilyName));

        // Assert
        Assert.AreEqual(2, includePaths.AsList().Count);
    }

    [TestMethod]
    public void ThenInclude_WithoutIncludeCalledFirst_ThrowsInvalidOperationException()
    {
        // Arrange
        var includePaths = (IIncludePathsBuilder<Person, Book>)Activator.CreateInstance(
            type: typeof(IncludePathsBuilder<Person, Book>),
            bindingAttr: BindingFlags.NonPublic | BindingFlags.Instance,
            binder: null,
            args: new[] { new List<string>() },
            culture: null)!;

        // Act
        var action = () => includePaths.ThenInclude(book => book.Authors);

        // Assert
        var exception = Assert.ThrowsException<InvalidOperationException>(action);
        Assert.AreEqual("The 'ThenInclude' method cannot be called before at least one path has been included.", exception.Message);
    }

    [TestMethod]
    public void ThenInclude_GivenNullExpression_ThrowsArgumentException()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        void action() => includePaths.ThenInclude(null!);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n\t")]
    public void ThenInclude_GivenNullOrEmptyOrWhiteSpaceString_ThrowsArgumentException(string path)
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        void action() => includePaths.ThenInclude(path);

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void ThenInclude_GivenNonExistentPath_IncludesIt()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includePaths.ThenInclude("UndefinedProperty");

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.FavoriteBook)}.UndefinedProperty"));
    }

    [TestMethod]
    public void ThenInclude_GivenUndefinedProperty_ThrowsArgumentException()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        void action() => includePaths.ThenInclude(p => (object)"UndefinedProperty");

        // Assert
        Assert.ThrowsException<ArgumentException>(action);
    }

    [TestMethod]
    public void ThenInclude_GivenValidPath_IncludesIt()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includePaths.ThenInclude(nameof(Book.Authors));

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
    }

    [TestMethod]
    public void ThenInclude_GivenValidProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includePaths.ThenInclude(book => book!.Title);

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Title)}"));
    }

    [TestMethod]
    public void ThenInclude_GivenValidCollectionProperty_IncludesExpectedPath()
    {
        // Arrange
        IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

        // Act
        includePaths.ThenInclude(book => book!.Authors);

        // Assert
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
    }

    [TestMethod]
    public void ThenInclude_WithPreviousProperty_IncludesIt()
    {
        // Arrange
        IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

        // Act
        includePaths
            .Include(p => p.Friends).ThenInclude(f => f.FavoriteBook)
            .Include(p => p.Friends).ThenInclude(f => f.Friends);

        // Assert
        Assert.AreEqual(2, includePaths.AsList().Count);
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.Friends)}.{nameof(Person.FavoriteBook)}"));
        Assert.IsTrue(includePaths.AsList().Contains($"{nameof(Person.Friends)}.{nameof(Person.Friends)}"));
    }
}
