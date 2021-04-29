// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
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
            services.AddDbContext<SampleDbContext>(
                builder => builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DevelopistCorePersistence_TestDb;Trusted_Connection=true;MultipleActiveResultSets=true"),
                ServiceLifetime.Scoped);
            services.AddPersistence<SampleDbContext>(lifetime: ServiceLifetime.Transient);

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SampleDbContext>();
            dbContext.Database.EnsureCreated();

            uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestCleanup]
        public void CleanUp()
        {
            uow.Dispose();
            (uow as EntityFramework.IUnitOfWork<SampleDbContext>)?.DbContext.Database.EnsureDeleted();
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
            await uow.BeginTransactionAsync().ConfigureAwait(false);

            // Act
            async Task action() => await uow.BeginTransactionAsync().ConfigureAwait(false);

            // Assert
            await Assert.ThrowsExceptionAsync<InvalidOperationException>(action).ConfigureAwait(false);
        }
    }
}
