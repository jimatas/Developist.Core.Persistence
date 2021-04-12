// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

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

        [TestMethod]
        public void Find_GivenNullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            var repository = uow.Repository<Person>();
            IQueryableFilter<Person> filter = null;

            // Act
            Action action = () => repository.Find(filter);

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
            Action action = () => repository.Find(filter, paginator);

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
            Action action = () => repository.Find(filter, paginator);

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
    }
}
