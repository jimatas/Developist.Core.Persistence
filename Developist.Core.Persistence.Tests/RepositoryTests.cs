// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class RepositoryTests
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

        #region Repository<TEntity>.Find tests
        [TestMethod]
        public void Find_GivenNullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = null;

            // Act
            void action() => repository.Find(filter);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Find_GivenNullFilterAndNonNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = null;
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>();

            // Act
            void action() => repository.Find(filter, paginator);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Find_GivenNonNullFilterAndNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = new PersonByIdFilter(default);
            IQueryablePaginator<Person> paginator = null;

            // Act
            void action() => repository.Find(filter, paginator);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Find_GivenNullIncludePaths_DoesNotThrow()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = new PersonByIdFilter(default);
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>();
            IEntityIncludePaths<Person> includePaths = null;

            // Act
            repository.Find(filter, includePaths);
            repository.Find(filter, paginator, includePaths);

            // Assert
        }

        [TestMethod]
        public void Find_GivenFilter_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByIdFilter(id: 2);

            // Act
            var people = repository.Find(filter);

            // Assert
            Assert.AreEqual(1, people.Count());
            Assert.AreEqual("Randall Bloom", people.Single().FullName());
        }

        [TestMethod]
        public void Find_GivenFilterAndPaginator_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByIdFilter(id: 2);
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>(nameof(Person.FamilyName), 1, 1);

            // Act
            var people = repository.Find(filter, paginator);

            // Assert
            Assert.AreEqual(1, people.Count());
            Assert.AreEqual("Randall Bloom", people.Single().FullName());
        }
        #endregion

        #region Repository<TEntity>.FindAsync tests
        [TestMethod]
        public async Task FindAsync_GivenNullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = null;

            // Act
            async Task action() => await repository.FindAsync(filter);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNullFilterAndNonNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = null;
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>();

            // Act
            async Task action() => await repository.FindAsync(filter, paginator);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNonNullFilterAndNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = new PersonByIdFilter(default);
            IQueryablePaginator<Person> paginator = null;

            // Act
            async Task action() => await repository.FindAsync(filter, paginator);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNullIncludePaths_DoesNotThrow()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = new PersonByIdFilter(default);
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>();
            IEntityIncludePaths<Person> includePaths = null;

            // Act
            await repository.FindAsync(filter, includePaths);
            await repository.FindAsync(filter, paginator, includePaths);

            // Assert
        }

        [TestMethod]
        public async Task FindAsync_GivenFilter_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByIdFilter(id: 2);

            // Act
            var people = await repository.FindAsync(filter);

            // Assert
            Assert.AreEqual(1, people.Count());
            Assert.AreEqual("Randall Bloom", people.Single().FullName());
        }

        [TestMethod]
        public async Task FindAsync_GivenFilterAndPaginator_ReturnsExpectedResult()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByIdFilter(id: 2);
            IQueryablePaginator<Person> paginator = new SorterPaginator<Person>(nameof(Person.FamilyName), 1, 1);

            // Act
            var people = await repository .FindAsync(filter, paginator);

            // Assert
            Assert.AreEqual(1, people.Count());
            Assert.AreEqual("Randall Bloom", people.Single().FullName());
        }
        #endregion

        #region Repository<TEntity>.Add tests
        [TestMethod]
        public void Add_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Person entity = null;

            // Act
            void action() => repository.Add(entity);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Add_WithoutComplete_DoesNotCommit()
        {
            // Arrange
            var repository = uow.Repository<Person>() as InMemory.Repository<Person>;
            var entity = new Person(4) { GivenName = "Dwayne", FamilyName = "Welsh" };

            // Act
            var countBefore = repository.DataStore.Count;
            repository.Add(entity);
            var countAfter = repository.DataStore.Count;

            // Assert
            Assert.AreEqual(0, countBefore);
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public void Add_WithComplete_Commits()
        {
            // Arrange
            var repository = uow.Repository<Person>() as InMemory.Repository<Person>;
            var entity = new Person(4) { GivenName = "Dwayne", FamilyName = "Welsh" };

            // Act
            var countBefore = repository.DataStore.Count;
            repository.Add(entity);
            uow.Complete();
            var countAfter = repository.DataStore.Count;

            // Assert
            Assert.AreNotEqual(countBefore, countAfter);
        }
        #endregion

        #region Repository<TEntity>.Remove tests
        [TestMethod]
        public void Remove_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            Person entity = null;

            // Act
            void action() => repository.Remove(entity);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public void Remove_WithoutComplete_DoesNotCommit()
        {
            // Arrange
            var repository = uow.Repository<Person>() as InMemory.Repository<Person>;
            SeedRepositoryWithData(repository);
            var entity = repository.Find(new PersonByIdFilter(1)).Single();

            // Act
            var countBefore = repository.DataStore.Count;
            repository.Remove(entity);
            var countAfter = repository.DataStore.Count;

            // Assert
            Assert.AreEqual(3, countBefore);
            Assert.AreEqual(countBefore, countAfter);
        }

        [TestMethod]
        public void Remove_WithComplete_Commits()
        {
            // Arrange
            var repository = uow.Repository<Person>() as InMemory.Repository<Person>;
            SeedRepositoryWithData(repository);
            var entity = repository.Find(new PersonByIdFilter(1)).Single();

            // Act
            var countBefore = repository.DataStore.Count;
            repository.Remove(entity);
            uow.Complete();
            var countAfter = repository.DataStore.Count;

            // Assert
            Assert.AreNotEqual(countBefore, countAfter);
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
