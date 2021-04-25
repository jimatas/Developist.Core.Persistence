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
    }
}
