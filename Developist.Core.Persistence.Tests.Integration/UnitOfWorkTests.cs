// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection;
using Developist.Core.Persistence.Tests.Integration.Fixture;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests.Integration
{
    [TestClass]
    public class UnitOfWorkTests
    {
        private IServiceProvider serviceProvider;
        private IUnitOfWork uow;

        [TestInitialize]
        public void Initialize()
        {
            var services = new ServiceCollection();
            services.AddLogging(logging => logging.AddConsole());
            services.AddDbContext<SampleDbContext>(
                dbContext => dbContext.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DevelopistCorePersistence_TestDb;Trusted_Connection=true;MultipleActiveResultSets=true"),
                ServiceLifetime.Scoped);
            services.AddUnitOfWork<SampleDbContext>(serviceLifetime: ServiceLifetime.Scoped);

            serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SampleDbContext>();
            dbContext.Database.EnsureCreated();

            uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestCleanup]
        public void CleanUp()
        {
            uow.Dispose();
            (uow as EntityFrameworkCore.IUnitOfWork<SampleDbContext>)?.DbContext.Database.EnsureDeleted();
            (serviceProvider as IDisposable)?.Dispose();
        }

        [TestMethod]
        public void BeginTransaction_CalledTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            uow.BeginTransaction();

            // Act
            void action() => uow.BeginTransaction();

            // Assert
            Assert.ThrowsException<InvalidOperationException>(action);
        }

        [TestMethod]
        public async Task BeginTransactionAsync_CalledTwice_ThrowsInvalidOperationException()
        {
            // Arrange
            await uow.BeginTransactionAsync();

            // Act
            async Task action() => await uow.BeginTransactionAsync();

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action);
        }
    }
}
