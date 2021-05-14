// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class SortPropertyTests
    {
        [TestMethod]
        public void Initialize_GivenNullPropertyName_ThrowsArgumentNullException()
        {
            // Arrange
            string propertyName = null;

            // Act
            void action() => new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("")]
        [DataRow(" \r\n")]
        public void Initialize_GivenEmptyOrWhitespacePropertyName_ThrowsArgumentNullException(string propertyName)
        {
            // Arrange
            
            // Act
            void action() => new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void Initialize_GivenNullPropertyExpression_ThrowsArgumentNullException()
        {
            // Arrange
            Expression<Func<Person, object>> propertySelector = null;

            // Act
            void action() => new SortProperty<Person, object>(propertySelector, SortDirection.Ascending);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Initialize_GivenInvalidPropertyName_DoesNotThrow()
        {
            // Arrange
            var propertyName = "UndefinedProperty";

            // Act
            var property = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.IsNotNull(property.Property);
        }

        [TestMethod]
        public void Initialize_GivenValidPropertyName_SetsPropertyExpression()
        {
            // Arrange
            var propertyName = "FamilyName";

            // Act
            var property = new SortProperty<Person>(propertyName, SortDirection.Ascending);

            // Assert
            Assert.IsNotNull(property.Property);
        }
    }
}
