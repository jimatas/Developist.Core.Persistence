// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortingPaginatorExtensionsTests
    {
        [TestMethod]
        public void HasNextPage_ByDefault_ReturnsFalse()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.HasNextPage();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasNextPage_AfterPaginating_ReturnsTrue()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);

            // Act
            var result = paginator.HasNextPage();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasNextPage_AfterPaginatingStartingAtLastPage_ReturnsFalse()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);
            paginator.PageNumber = paginator.PageCount;

            // Act
            var result = paginator.HasNextPage();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasPreviousPage_ByDefault_ReturnsFalse()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.HasPreviousPage();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void HasPreviousPage_AfterPaginatingStartingAtLastPage_ReturnsTrue()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);
            paginator.PageNumber = paginator.PageCount;

            // Act
            var result = paginator.HasPreviousPage();

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void HasPreviousPage_AfterPaginatingStartingAtFirstPage_ReturnsFalse()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);

            // Act
            var result = paginator.HasPreviousPage();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MoveNextPage_ByDefault_DoesNotMoveToNextPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.MoveNextPage();

            // Assert
            Assert.IsFalse(result);
            Assert.AreNotEqual(2, paginator.PageNumber);
        }

        [TestMethod]
        public void MoveNextPage_AfterPaginating_MovesToNextPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);

            // Act
            var result = paginator.MoveNextPage();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, paginator.PageNumber);
        }

        [TestMethod]
        public void MovePreviousPage_ByDefault_DoesNotMoveToPreviousPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.MovePreviousPage();

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MovePreviousPage_AfterPaginatingStartingAtLastPage_MovesToPreviousPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);
            paginator.Paginate(People);
            paginator.PageNumber = paginator.PageCount;

            // Act
            var result = paginator.MovePreviousPage();

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(paginator.PageCount - 1, paginator.PageNumber);
        }

        private static IQueryable<Person> People =>
            new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh" },
                new Person { GivenName = "Ed", FamilyName = "Stuart" },
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
                new Person { GivenName = "Glenn", FamilyName = "Hensley" },
                new Person { GivenName = "Phillipa", FamilyName = "Connor" },
                new Person { GivenName = "Peter", FamilyName = "Connor" },
                new Person { GivenName = "Ana", FamilyName = "Bryan" },
                new Person { GivenName = "Edgar", FamilyName = "Bernard" },
                new Person { GivenName = "Breanna", FamilyName = "Meyers" },
                new Person { GivenName = "Leroy", FamilyName = "Caldwell" },
                new Person { GivenName = "Ellis", FamilyName = "Rowland" },
                new Person { GivenName = "Josh", FamilyName = "Beltran" },
                new Person { GivenName = "Melissa", FamilyName = "Smith" },
                new Person { GivenName = "Donovan", FamilyName = "Webb" }

            }.AsQueryable();
    }
}
