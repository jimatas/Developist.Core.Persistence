// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SorterPaginatorTests
    {
        [TestMethod]
        public void NewInstance_ByDefault_SortdAscending()
        {
            // Arrange
            var paginator = new SorterPaginator<object>();

            // Act
            var isDescending = paginator.SortDescending;

            // Assert
            Assert.IsFalse(isDescending);
        }

        [TestMethod]
        public void PageSize_ByDefault_ReturnsDefaultPageSize()
        {
            // Arrange
            var paginator = new SorterPaginator<object>();

            // Act
            var pageSize = paginator.PageSize;

            // Assert
            Assert.AreEqual(SorterPaginator<object>.DefaultPageSize, pageSize);
        }

        [DataTestMethod]
        [DataRow(0)]
        [DataRow(-1)]
        [DataRow(int.MinValue)]
        public void PageNumber_GivenInvalidValue_ThrowsArgumentOutOfRangeException(int pageNumber)
        {
            // Arrange
            var paginator = new SorterPaginator<object>();

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
            var paginator = new SorterPaginator<object>();

            // Act
            Action action = () => paginator.PageSize = pageSize;

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void SortProperty_ByDefault_SetsSortExpression()
        {
            // Arrange
            var paginator = new SorterPaginator<Person>();

            // Act
            var isNullBefore = paginator.SortExpression is null;
            paginator.SortProperty = "FavoriteBook.Genre";
            var isNullAfter = paginator.SortExpression is null;

            // Assert
            Assert.IsTrue(isNullBefore);
            Assert.IsFalse(isNullAfter);
        }

        [TestMethod]
        public void SortProperty_GivenNull_ClearsPreviousSortExpression()
        {
            // Arrange
            var paginator = new SorterPaginator<Person>();
            paginator.SortProperty = "FavoriteBook.Genre";

            // Act
            var isNullBefore = paginator.SortExpression is null;
            paginator.SortProperty = null;
            var isNullAfter = paginator.SortExpression is null;

            // Assert
            Assert.IsFalse(isNullBefore);
            Assert.IsTrue(isNullAfter);
        }

        [TestMethod]
        public void Paginate_SortedByGivenName_ReturnsExpectedResult()
        {
            // Arrange
            var sequence = new[]
            {
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
                new Person { GivenName = "Glen", FamilyName = "Hensley" },

            }.AsQueryable();

            var paginator = new SorterPaginator<Person>("GivenName", pageNumber: 1, pageSize: 1);

            // Act
            var result = paginator.Paginate(sequence);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual("Glen", result.Single().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByFamilyNameAndPageSizeOf5_ReturnsExpectedResult()
        {
            // Arrange
            var sequence = new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh" },
                new Person { GivenName = "Ed", FamilyName = "Stuart" },
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
                new Person { GivenName = "Glenn", FamilyName = "Hensley" },
                new Person { GivenName = "Phillipa", FamilyName = "Connor" },
                new Person { GivenName = "Ana", FamilyName = "Bryan" },
                new Person { GivenName = "Edgar", FamilyName = "Bernard" }

            }.AsQueryable();

            var paginator = new SorterPaginator<Person>("FamilyName", pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.Paginate(sequence);

            // Assert
            Assert.AreEqual(5, result.Count());
            Assert.AreEqual("Bernard", result.First().FamilyName);
            Assert.AreEqual("Hensley", result.Last().FamilyName);
        }
    }
}
