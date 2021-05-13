// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortingPaginatorTests
    {
        [TestMethod]
        public void PageSize_ByDefault_ReturnsDefaultPageSize()
        {
            // Arrange
            var paginator = new SortingPaginator<object>();

            // Act
            var pageSize = paginator.PageSize;

            // Assert
            Assert.AreEqual(SortingPaginator<object>.DefaultPageSize, pageSize);
        }

        [TestMethod]
        public void PageNumber_ByDefault_ReturnsOne()
        {
            // Arrange
            var paginator = new SortingPaginator<object>();

            // Act
            var pageNumber = paginator.PageNumber;

            // Assert
            Assert.AreEqual(1, pageNumber);
        }

        [TestMethod]
        public void PageCount_ByDefault_ReturnsZero()
        {
            // Arrange
            var paginator = new SortingPaginator<object>();

            // Act
            var pageCount = paginator.PageCount;

            // Assert
            Assert.AreEqual(0, pageCount);
        }

        [TestMethod]
        public void ItemCount_ByDefault_ReturnsZero()
        {
            // Arrange
            var paginator = new SortingPaginator<object>();

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
            var paginator = new SortingPaginator<object>();

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
            var paginator = new SortingPaginator<object>();

            // Act
            void action() => paginator.PageSize = pageSize;

            // Assert
            Assert.ThrowsException<ArgumentOutOfRangeException>(action);
        }

        [TestMethod]
        public void Paginate_WithNoSortProperties_ReturnsUnorderedPage()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(5, result.Count());
            Assert.AreEqual("Welsh", result.First().FamilyName);
            Assert.AreEqual("Hensley", result.Last().FamilyName);
        }

        [TestMethod]
        public void Paginate_SortedByGivenNameAscendingUsingExpression_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2);
            paginator.SortProperties.Add(new SortProperty<Person>(p => p.GivenName, SortDirection.Ascending));

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Ana", result.First().GivenName);
            Assert.AreEqual("Dwayne", result.Last().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByGivenNameAscending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2);
            paginator.SortProperties.Add(new SortProperty<Person>("GivenName", SortDirection.Ascending));

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Ana", result.First().GivenName);
            Assert.AreEqual("Dwayne", result.Last().GivenName);
        }

        [TestMethod]
        public void Paginate_SortedByFamilyNameDescending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2);
            paginator.SortProperties.Add(new SortProperty<Person>("FamilyName", SortDirection.Descending));

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Welsh", result.First().FamilyName);
            Assert.AreEqual("Stuart", result.Last().FamilyName);
        }

        [TestMethod]
        public void Paginate_SortedByFamilyNameAscendingThenGivenNameDescending_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>(pageNumber: 2, pageSize: 3);
            paginator.SortProperties.Add(new SortProperty<Person>("FamilyName", SortDirection.Ascending));
            paginator.SortProperties.Add(new SortProperty<Person>("GivenName", SortDirection.Descending));

            // Act
            var result = paginator.Paginate(People);

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
            var paginator = new SortingPaginator<Person>().StartingAt(1).WithPageSizeOf(3);
            paginator.SortedBy("Age", SortDirection.Ascending);

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual("Phillipa Connor", result.First().FullName()); // null < any other value
            Assert.AreEqual("Peter Connor", result.ElementAt(1).FullName());
            Assert.AreEqual("Dwayne Welsh", result.Last().FullName());
        }

        [TestMethod]
        public void Paginate_SortedByAgeDescendingUsingExpression_ReturnsExpectedResult()
        {
            // Arrange
            var paginator = new SortingPaginator<Person>().StartingAt(1).WithPageSizeOf(2);
            paginator.SortedBy(p => p.Age, SortDirection.Descending);

            // Act
            var result = paginator.Paginate(People);

            // Assert
            Assert.AreEqual(2, result.Count());
            Assert.AreEqual("Edgar Bernard", result.First().FullName());
            Assert.AreEqual("Randall Bloom", result.Last().FullName());
        }

        private static IQueryable<Person> People =>
            new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh", Age = 18 },
                new Person { GivenName = "Ed", FamilyName = "Stuart", Age = 24 },
                new Person { GivenName = "Hollie", FamilyName = "Marin", Age = 36 },
                new Person { GivenName = "Randall", FamilyName = "Bloom", Age = 55 },
                new Person { GivenName = "Glenn", FamilyName = "Hensley", Age = 27 },
                new Person { GivenName = "Phillipa", FamilyName = "Connor", Age = null },
                new Person { GivenName = "Peter", FamilyName = "Connor", Age = 12 },
                new Person { GivenName = "Ana", FamilyName = "Bryan", Age = 44 },
                new Person { GivenName = "Edgar", FamilyName = "Bernard", Age = 80 }

            }.AsQueryable();
    }
}
