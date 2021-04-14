// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class UnitOfWorkTests
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
        public void Repository_GivenValidGenericType_ReturnsRepository()
        {
            // Arrange

            // Act
            var personRepository = uow.Repository<Person>();

            // Assert
            Assert.IsNotNull(personRepository);
        }

        [TestMethod]
        public void Repository_CalledTwiceForSameGenericType_ReturnsSameRepository()
        {
            // Arrange

            // Act
            var personRepository = uow.Repository<Person>();
            var anotherPersonRepository = uow.Repository<Person>();

            // Assert
            Assert.AreEqual(personRepository, anotherPersonRepository);
        }

        [TestMethod]
        public void Repository_CalledTwiceForDifferentGenericTypes_ReturnsTwoRepositories()
        {
            // Arrange

            // Act
            var bookRepository = uow.Repository<Book>();
            var personRepository = uow.Repository<Person>();

            // Assert
            Assert.AreNotEqual(bookRepository, personRepository);
        }

        [TestMethod]
        public void Complete_ByDefault_FiresCompletedEvent()
        {
            // Arrange
            var isCompleted = false;
            uow.Completed += (sender, e) => isCompleted = true;

            // Act
            uow.Complete();

            // Assert
            Assert.IsTrue(isCompleted);
        }

        [TestMethod]
        public async Task CompleteAsync_ByDefault_FiresCompletedEvent()
        {
            // Arrange
            var isCompleted = false;
            uow.Completed += (sender, e) => isCompleted = true;

            // Act
            await uow.CompleteAsync().ConfigureAwait(false);

            // Assert
            Assert.IsTrue(isCompleted);
        }
    }
}
