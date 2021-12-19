// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Pagination;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortingPaginatorExtensionsTests
    {
        [TestMethod]
        public void SortedBy_GivenInvalidProperty_ThrowsArgumentException()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedBy("UndefinedProperty");

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Person'. (Parameter 'propertyName')", exception.Message);
        }

        [TestMethod]
        public void SortedBy_GivenInvalidNestedProperty_ThrowsArgumentException()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedBy("FavoriteBook.UndefinedProperty");

            // Assert
            var exception = Assert.ThrowsException<ArgumentException>(action);
            Assert.AreEqual("No property 'UndefinedProperty' on type 'Book'. (Parameter 'propertyName')", exception.Message);
        }

        [TestMethod]
        public void SortedByString_GivenNullString_ThrowsArgumentNullException()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedByString(null);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" \r\n")]
        public void SortedByString_GivenEmptyOrWhiteSpaceOnlyString_ThrowsArgumentException(string value)
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedByString(value);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [DataTestMethod]
        [DataRow("+()")]
        [DataRow("GivenName,-( )")]
        public void SortedByString_GivenInvalidString_ThrowsFormatException(string value)
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            void action() => paginator.SortedByString(value);

            // Assert
            Assert.ThrowsException<FormatException>(action);
        }

        [TestMethod]
        public void SortedByString_GivenSingleDirective_AddsSortDirective()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            paginator.SortedByString("FamilyName");

            // Assert
            Assert.AreEqual(1, paginator.SortDirectives.Count);
            Assert.AreEqual("FamilyName", GetPropertyNameFromLambda(((SortProperty<Person>)paginator.SortDirectives.First()).Property));
            Assert.AreEqual(SortDirection.Ascending, ((SortProperty<Person>)paginator.SortDirectives.First()).Direction);
        }

        [TestMethod]
        public void SortedByString_GivenTwoDirectives_AddsBoth()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>();

            // Act
            paginator.SortedByString("FamilyName,-Age");

            // Assert
            Assert.AreEqual(2, paginator.SortDirectives.Count);
            Assert.AreEqual("FamilyName", GetPropertyNameFromLambda(((SortProperty<Person>)paginator.SortDirectives.First()).Property));
            Assert.AreEqual(SortDirection.Ascending, ((SortProperty<Person>)paginator.SortDirectives.First()).Direction);
            Assert.AreEqual("Age", GetPropertyNameFromLambda(((SortProperty<Person>)paginator.SortDirectives.Last()).Property));
            Assert.AreEqual(SortDirection.Descending, ((SortProperty<Person>)paginator.SortDirectives.Last()).Direction);
        }

        private static string GetPropertyNameFromLambda(LambdaExpression expression)
        {
            return (expression.Body as MemberExpression)?.Member.Name;
        }
    }
}
