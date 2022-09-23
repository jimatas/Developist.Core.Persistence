using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.Tests.Fixture;

using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
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
            Assert.IsFalse(includePaths.ToArray().Any());
        }

        [TestMethod]
        public void Include_GivenNullString_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            void action() => includePaths.Include(null!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenNullExpression_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            void action() => includePaths.Include((Expression<Func<Person, Book>>)null!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\r\n\t")]
        public void Include_GivenEmptyOrWhiteSpaceString_ThrowsArgumentException(string path)
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
            Assert.IsTrue(includePaths.ToArray().Contains("UndefinedProperty"));
        }

        [TestMethod]
        public void Include_GivenUndefinedProperty_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            void action() => includePaths.Include(p => (object)"UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenValidPath_IncludesIt()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            includePaths.Include(nameof(Person.FamilyName));

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Include_GivenValidProperty_IncludesExpectedPath()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName);

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Include_GivenValidCollectionProperty_IncludesExpectedPath()
        {
            // Arrange
            IIncludePathsBuilder<Person> includePaths = new IncludePathsBuilder<Person>();

            // Act
            includePaths = includePaths.Include(person => person.Friends);

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains(nameof(Person.Friends)));
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
            Assert.AreEqual(2, includePaths.ToArray().Length);
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
            Assert.AreEqual(2, includePaths.ToArray().Length);
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
            Assert.AreEqual(2, includePaths.ToArray().Length);
        }

        [TestMethod]
        public void ThenInclude_GivenNullString_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            void action() => includePaths.ThenInclude(null!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ThenInclude_GivenNullExpression_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            void action() => includePaths.ThenInclude(null!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" ")]
        [DataRow("\r\n\t")]
        public void ThenInclude_GivenEmptyOrWhiteSpaceString_ThrowsArgumentException(string path)
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
            Assert.IsTrue(includePaths.ToArray().Contains($"{nameof(Person.FavoriteBook)}.UndefinedProperty"));
        }

        [TestMethod]
        public void ThenInclude_GivenUndefinedProperty_ThrowsArgumentNullException()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            void action() => includePaths.ThenInclude(p => (object)"UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ThenInclude_GivenValidPath_IncludesIt()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            includePaths.ThenInclude(nameof(Book.Authors));

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
        }

        [TestMethod]
        public void ThenInclude_GivenValidProperty_IncludesExpectedPath()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            includePaths.ThenInclude(book => book!.Title);

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Title)}"));
        }

        [TestMethod]
        public void ThenInclude_GivenValidCollectionProperty_IncludesExpectedPath()
        {
            // Arrange
            IIncludePathsBuilder<Person, Book?> includePaths = new IncludePathsBuilder<Person>().Include(p => p.FavoriteBook);

            // Act
            includePaths.ThenInclude(book => book!.Authors);

            // Assert
            Assert.IsTrue(includePaths.ToArray().Contains($"{nameof(Person.FavoriteBook)}.{nameof(Book.Authors)}"));
        }
    }
}
