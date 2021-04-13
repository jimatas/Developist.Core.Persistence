// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class RepositoryExtensionsTests
    {
        private IUnitOfWork uow;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();
            services.AddLogging(config => config.AddConsole());

            services.Add(new ServiceDescriptor(typeof(IRepositoryFactory), typeof(InMemory.RepositoryFactory), ServiceLifetime.Scoped));
            services.Add(new ServiceDescriptor(typeof(IUnitOfWork), typeof(InMemory.UnitOfWork), ServiceLifetime.Scoped));

            uow = services.BuildServiceProvider().GetRequiredService<IUnitOfWork>();
        }

        [TestMethod]
        public void EnsureUnitOfWorkRegistered()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsNotNull(uow);
        }

        [TestMethod]
        public void Find_GivenUnknownId_ReturnsNull()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Find(id: 0);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Find_GivenValidId_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Find(id: 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Glen Hensley", result.FullName());
        }

        [TestMethod]
        public void Find_GivenNullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> predicate = null;

            // Act
            Action action = () => repository.Find(predicate: predicate);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Find_GivenPredicateThatMatchesNothing_ReturnsEmptyResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> predicate = p => false;

            // Act
            var result = repository.Find(predicate);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public void Find_GivenPredicateMatchingOne_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);
            Expression<Func<Person, bool>> predicate = p => p.GivenName.Equals("Hollie");

            // Act
            var result = repository.Find(predicate);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Single().Id);
        }

        [TestMethod]
        public void Find_GivenPredicateMatchingMultiple_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);
            Expression<Func<Person, bool>> predicate = p => p.GivenName.Contains("ll");

            // Act
            var result = repository.Find(predicate);

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        private static void SeedRepositoryWithData(IRepository<Person> repository)
        {
            foreach (var person in new[]
            {
                new Person(1) { GivenName = "Hollie", FamilyName = "Marin" },
                new Person(2) { GivenName = "Randall", FamilyName = "Bloom" },
                new Person(3) { GivenName = "Glen", FamilyName = "Hensley" }
            })
            {
                repository.Add(person);
            }
            repository.UnitOfWork.Complete();
        }
    }
}
