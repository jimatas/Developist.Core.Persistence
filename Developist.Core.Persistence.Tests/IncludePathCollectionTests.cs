// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class IncludePathCollectionTests
    {
        [TestMethod]
        public void NewInstance_ByDefault_HasNoPaths()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act

            // Assert
            Assert.IsFalse(includePaths.Any());
        }

        #region IncludePathCollection.Add tests
        [TestMethod]
        public void Add_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Add(null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Add_GivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Add(string.Empty);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Add_GivenValidPath_AddsPath()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths.Add(nameof(Person.FamilyName));

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Add_GivenInvalidPath_StillAddsPath()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths.Add("UndefinedProperty");

            // Assert
            Assert.IsTrue(includePaths.Contains("UndefinedProperty"));
        }

        [TestMethod]
        public void Add_GivenSamePathTwice_AddsBothPaths()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths.Add(nameof(Person.FamilyName));
            includePaths.Add(nameof(Person.FamilyName));

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }

        [TestMethod]
        public void Add_GivenTwoDifferentPaths_AddsBothPaths()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths.Add(nameof(Person.GivenName));
            includePaths.Add(nameof(Person.FamilyName));

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }
        #endregion

        #region IncludePathCollection.Remove tests
        [TestMethod]
        public void Remove_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Remove(null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Remove_GivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Remove(string.Empty);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Remove_GivenExistingPath_RemovesPath()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();
            includePaths.Add(nameof(Person.FamilyName));

            // Act
            includePaths.Remove(nameof(Person.FamilyName));

            // Assert
            Assert.IsFalse(includePaths.Any());
        }

        [TestMethod]
        public void Remove_GivenExistingPathWithDuplicates_RemovesOnlyOne()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();
            includePaths.Add(nameof(Person.FamilyName));
            includePaths.Add(nameof(Person.FamilyName));

            // Act
            includePaths.Remove(nameof(Person.FamilyName));

            // Assert
            Assert.AreEqual(1, includePaths.Count());
        }

        [TestMethod]
        public void Remove_GivenExistingPathWithDuplicates_RemovesLastOccurrence()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();
            includePaths.Add(nameof(Person.FamilyName));
            includePaths.Add(nameof(Person.GivenName));
            includePaths.Add(nameof(Person.FamilyName)); // Duplicated

            // Act
            includePaths.Remove(nameof(Person.FamilyName));

            // Assert
            Assert.AreEqual(nameof(Person.FamilyName), includePaths.ElementAt(0));
            Assert.AreEqual(nameof(Person.GivenName), includePaths.ElementAt(1));
        }
        #endregion

        #region IncludePathCollection.Include tests
        [TestMethod]
        public void Include_CalledOnce_AddsOnePath()
        {
            // Arrange
            IIncludePathCollection<Person> includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(1, includePaths.Count());
        }

        [TestMethod]
        public void Include_GivenSimpleProperty_AddsExpectedPath()
        {
            // Arrange
            IIncludePathCollection<Person> includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Include_CalledTwiceForSameProperty_AddsBothPaths()
        {
            // Arrange
            IIncludePathCollection<Person> includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName).Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }

        [TestMethod]
        public void Include_CalledTwiceForDifferentProperties_AddsBothPaths()
        {
            // Arrange
            IIncludePathCollection<Person> includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths = includePaths.Include(person => person.GivenName).Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }

        [TestMethod]
        public void Include_GivenObjectProperty_AddsExpectedPath()
        {
            // Arrange
            IIncludePathCollection<Person> includePaths = new IncludePathCollection<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FavoriteBook);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Person.FavoriteBook)));
        }

        [TestMethod]
        public void Include_GivenCollectionProperty_AddsExpectedPath()
        {
            // Arrange
            IIncludePathCollection<Book> includePaths = new IncludePathCollection<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Authors);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Authors)));
        }

        [TestMethod]
        public void Include_GivenNullProperty_ThrowsException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Include(person => (string)null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenUndefinedProperty_ThrowsException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            void action() => includePaths.Include(person => "UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenNullPropertySelector_ThrowsException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();
            Expression<Func<Person, string>> propertySelector = null;

            // Act
            void action() => includePaths.Include(propertySelector);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }
        #endregion

        #region IncludePathCollection.ThenInclude tests
        [TestMethod]
        public void ThenInclude_GivenUndefinedProperty_ThrowsException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();

            // Act
            Action action = () => includePaths.Include(person => person.FavoriteBook).ThenInclude(b => "UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ThenInclude_GivenNullPropertySelector_ThrowsException()
        {
            // Arrange
            var includePaths = new IncludePathCollection<Person>();
            Expression<Func<Book, Person>> propertySelector = null;

            // Act
            Action action = () => includePaths.Include(person => person.FavoriteBook).ThenInclude(propertySelector);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ThenInclude_GivenNestedProperty_AddsExpectedPath()
        {
            // Arrange
            IIncludePathCollection<Book> includePaths = new IncludePathCollection<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Authors).ThenInclude(author => author.FavoriteBook);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Authors) + '.' + nameof(Person.FavoriteBook)));
        }

        [TestMethod]
        public void ThenInclude_GivenNestedPropertyWithSomePathAlreadySet_RetainsThatPath()
        {
            // Arrange
            IIncludePathCollection<Book> includePaths = new IncludePathCollection<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Title).Include(book => book.Authors).ThenInclude(author => author.FavoriteBook);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Title)));
        }
        #endregion
    }
}
