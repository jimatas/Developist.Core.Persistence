// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.InMemory.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class RepositoryExtensionsTests
    {
        private IServiceProvider serviceProvider;
        private IUnitOfWork uow;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                .AddUnitOfWork();

            serviceProvider = services.BuildServiceProvider();
            uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestCleanup]
        public void CleanUp() => (serviceProvider as IDisposable)?.Dispose();

        [TestMethod]
        public void EnsureUnitOfWorkRegistered()
        {
            // Arrange

            // Act

            // Assert
            Assert.IsNotNull(uow);
        }

        #region RepositoryExtensions.Count tests
        [TestMethod]
        public void Count_GivenNullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> nullPredicate = null;

            // Act
            void action() => repository.Count(predicate: nullPredicate);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Count_GivenPredicateMatchingOne_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Count(predicate: p => p.FamilyName.Equals("Bloom"));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void Count_GivenPredicateMatchingMultiple_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Count(predicate: p => p.FamilyName.Equals("Bloom") || p.FamilyName.Equals("Marin"));

            // Assert
            Assert.AreEqual(2, result);
        }
        #endregion

        #region RepositoryExtensions.CountAsync tests
        [TestMethod]
        public async Task CountAsync_GivenNullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> nullPredicate = null;

            // Act
            async Task action() => await repository.CountAsync(predicate: nullPredicate);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task CountAsync_GivenPredicateMatchingOne_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = await repository.CountAsync(predicate: p => p.FamilyName.Equals("Bloom"));

            // Assert
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public async Task CountAsync_GivenPredicateMatchingMultiple_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = await repository.CountAsync(predicate: p => p.FamilyName.Equals("Bloom") || p.FamilyName.Equals("Marin"));

            // Assert
            Assert.AreEqual(2, result);
        }
        #endregion

        #region RepositoryExtensions.Get tests
        [TestMethod]
        public void Get_GivenUnknownId_ReturnsNull()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Get(id: 0);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Get_GivenValidId_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = repository.Get(id: 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Glen Hensley", result.FullName());
        }
        #endregion

        #region RepositoryExtensions.GetAsync tests
        [TestMethod]
        public async Task GetAsync_GivenUnknownId_ReturnsNull()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = await repository.GetAsync(id: 0);

            // Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public async Task GetAsync_GivenValidId_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            // Act
            var result = await repository.GetAsync(id: 3);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("Glen Hensley", result.FullName());
        }
        #endregion

        #region RepositoryExtensions.Find tests
        [TestMethod]
        public void Find_GivenNullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> predicate = null;

            // Act
            void action() => repository.Find(predicate: predicate);

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
        #endregion

        #region RepositoryExtensions.FindAsync tests
        [TestMethod]
        public async Task FindAsync_GivenNullPredicate_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> predicate = null;

            // Act
            async Task action() => await repository.FindAsync(predicate: predicate);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenPredicateThatMatchesNothing_ReturnsEmptyResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Expression<Func<Person, bool>> predicate = p => false;

            // Act
            var result = await repository.FindAsync(predicate);

            // Assert
            Assert.AreEqual(0, result.Count());
        }

        [TestMethod]
        public async Task FindAsync_GivenPredicateMatchingOne_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);
            Expression<Func<Person, bool>> predicate = p => p.GivenName.Equals("Hollie");

            // Act
            var result = await repository.FindAsync(predicate);

            // Assert
            Assert.AreEqual(1, result.Count());
            Assert.AreEqual(1, result.Single().Id);
        }

        [TestMethod]
        public async Task FindAsync_GivenPredicateMatchingMultiple_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);
            Expression<Func<Person, bool>> predicate = p => p.GivenName.Contains("ll");

            // Act
            var result = await repository.FindAsync(predicate);

            // Assert
            Assert.AreEqual(2, result.Count());
        }
        #endregion

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
