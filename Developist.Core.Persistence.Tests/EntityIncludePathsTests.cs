// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class EntityIncludePathsTests
    {
        #region EntityIncludePaths.ForEntity tests
        [TestMethod]
        public void ForEntity_ByDefault_HasNoPaths()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act

            // Assert
            Assert.IsFalse(includePaths.Any());
        }
        #endregion

        #region EntityIncludePaths.Add tests
        [TestMethod]
        public void Add_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Add(null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Add_GivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Add(string.Empty);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Add_GivenValidPath_AddsPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths.Add(nameof(Person.FamilyName));

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Add_GivenInvalidPath_StillAddsPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths.Add("UndefinedProperty");

            // Assert
            Assert.IsTrue(includePaths.Contains("UndefinedProperty"));
        }

        [TestMethod]
        public void Add_GivenSamePathTwice_AddsBothPaths()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

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
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths.Add(nameof(Person.GivenName));
            includePaths.Add(nameof(Person.FamilyName));

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }
        #endregion

        #region EntityIncludePaths.Remove tests
        [TestMethod]
        public void Remove_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Remove(null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Remove_GivenEmptyString_ThrowsArgumentException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Remove(string.Empty);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Remove_GivenExistingPath_RemovesPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();
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
            var includePaths = EntityIncludePaths.ForEntity<Person>();
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
            var includePaths = EntityIncludePaths.ForEntity<Person>();
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

        #region EntityIncludePaths.Include tests
        [TestMethod]
        public void Include_CalledOnce_AddsOnePath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(1, includePaths.Count());
        }

        [TestMethod]
        public void Include_GivenSimpleProperty_AddsExpectedPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Person.FamilyName)));
        }

        [TestMethod]
        public void Include_CalledTwiceForSameProperty_AddsBothPaths()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths = includePaths.Include(person => person.FamilyName).Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }

        [TestMethod]
        public void Include_CalledTwiceForDifferentProperties_AddsBothPaths()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            includePaths = includePaths.Include(person => person.GivenName).Include(person => person.FamilyName);

            // Assert
            Assert.AreEqual(2, includePaths.Count());
        }

        [TestMethod]
        public void Include_GivenObjectProperty_AddsExpectedPath()
        {
            // Arrange
            var include = EntityIncludePaths.ForEntity<Person>();

            // Act
            include = include.Include(person => person.FavoriteBook);

            // Assert
            Assert.IsTrue(include.Contains(nameof(Person.FavoriteBook)));
        }

        [TestMethod]
        public void Include_GivenCollectionProperty_AddsExpectedPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Authors);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Authors)));
        }

        [TestMethod]
        public void Include_GivenNullProperty_ThrowsException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Include(person => (string)null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenUndefinedProperty_ThrowsException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Include(person => "UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Include_GivenNullPropertySelector_ThrowsException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();
            Expression<Func<Person, string>> propertySelector = null;

            // Act
            Action action = () => includePaths.Include(propertySelector);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }
        #endregion

        #region EntityIncludePaths.ThenInclude tests
        [TestMethod]
        public void ThenInclude_GivenUndefinedProperty_ThrowsException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();

            // Act
            Action action = () => includePaths.Include(person => person.FavoriteBook).ThenInclude(b => "UndefinedProperty");

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void ThenInclude_GivenNullPropertySelector_ThrowsException()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Person>();
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
            var includePaths = EntityIncludePaths.ForEntity<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Authors).ThenInclude(author => author.FavoriteBook);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Authors) + '.' + nameof(Person.FavoriteBook)));
        }

        [TestMethod]
        public void ThenInclude_GivenNestedPropertyWithSomePathAlreadySet_RetainsThatPath()
        {
            // Arrange
            var includePaths = EntityIncludePaths.ForEntity<Book>();

            // Act
            includePaths = includePaths.Include(book => book.Title).Include(book => book.Authors).ThenInclude(author => author.FavoriteBook);

            // Assert
            Assert.IsTrue(includePaths.Contains(nameof(Book.Title)));
        }
        #endregion
    }
}
