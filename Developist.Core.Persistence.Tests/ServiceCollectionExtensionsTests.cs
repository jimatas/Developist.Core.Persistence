// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private IServiceCollection services = new ServiceCollection();

        #region AddPersistence tests
        [TestMethod]
        public void AddPersistence_ByDefault_RegistersInMemoryUnitOfWork()
        {
            // Arrange

            // Act
            services.AddPersistence();
            var serviceProvider = services.BuildServiceProvider();

            IUnitOfWork uow = serviceProvider.GetService<IUnitOfWork>();

            // Assert
            Assert.IsInstanceOfType(uow, typeof(InMemory.UnitOfWork));
        }

        [TestMethod]
        public void AddPersistence_GivenInvalidRepositoryFactoryType_ThrowsArgumentException()
        {
            // Arrange
            var invalidRepositoryFactoryType = typeof(object);

            // Act
            void action() => services.AddPersistence(invalidRepositoryFactoryType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddPersistence_GivenRepositoryFactoryInterfaceType_ThrowsArgumentException()
        {
            // Arrange
            var repositoryFactoryInterfaceType = typeof(IRepositoryFactory);

            // Act
            void action() => services.AddPersistence(repositoryFactoryInterfaceType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddPersistence_GivenValidRepositoryFactoryType_DoesNotThrow()
        {
            // Arrange
            var validRepositoryFactoryType = typeof(InMemory.RepositoryFactory);

            // Act
            void action() => services.AddPersistence(validRepositoryFactoryType);

            // Assert
            try
            {
                action();
            }
            catch
            {
                Assert.Fail();
            }
        }

        [TestMethod]
        public void AddPersistence_GivenValidRepositoryFactorySubtype_DoesNotThrow()
        {
            // Arrange
            var validRepositoryFactorySubtype = typeof(EntityFramework.RepositoryFactory<SampleDbContext>);

            // Act
            void action() => services.AddPersistence(validRepositoryFactorySubtype);

            // Assert
            try
            {
                action();
            }
            catch
            {
                Assert.Fail();
            }
        }
        #endregion

        #region AddPersistence<TDbContext> tests
        [TestMethod]
        public void AddPersistenceOfTDbContext_ByDefault_RegistersEntityFrameworkUnitOfWork()
        {
            // Arrange
            services.AddScoped<SampleDbContext, SampleDbContext>();

            // Act
            services.AddPersistence<SampleDbContext>();
            var serviceProvider = services.BuildServiceProvider();

            IUnitOfWork uow = serviceProvider.GetService<IUnitOfWork>();

            // Assert
            Assert.IsInstanceOfType(uow, typeof(EntityFramework.UnitOfWork<SampleDbContext>));
        }

        [TestMethod]
        public void AddPersistenceOfTDbContext_GivenInvalidRepositoryFactoryType_ThrowsArgumentException()
        {
            // Arrange
            var invalidRepositoryFactoryType = typeof(object);

            // Act
            void action() => services.AddPersistence<SampleDbContext>(invalidRepositoryFactoryType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddPersistenceOfTDbContext_GivenRepositoryFactoryInterfaceType_ThrowsArgumentException()
        {
            // Arrange
            var repositoryFactoryInterfaceType = typeof(EntityFramework.IRepositoryFactory<SampleDbContext>);

            // Act
            void action() => services.AddPersistence<SampleDbContext>(repositoryFactoryInterfaceType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddPersistenceOfTDbContext_GivenValidRepositoryFactoryType_DoesNotThrow()
        {
            // Arrange
            var validRepositoryFactoryType = typeof(EntityFramework.RepositoryFactory<SampleDbContext>);

            // Act
            void action() => services.AddPersistence(validRepositoryFactoryType);

            // Assert
            try
            {
                action();
            }
            catch
            {
                Assert.Fail();
            }
        }
        #endregion
    }
}
