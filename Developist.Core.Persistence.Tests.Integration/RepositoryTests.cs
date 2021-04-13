// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;

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

            services.AddDbContext<SampleDbContext>(
                builder => builder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DevelopistCorePersistence_TestDb;Trusted_Connection=true;MultipleActiveResultSets=true"),
                ServiceLifetime.Scoped);

            services.AddScoped<EntityFramework.IRepositoryFactory<SampleDbContext>, EntityFramework.RepositoryFactory<SampleDbContext>>();
            services.AddScoped<IRepositoryFactory>(provider => provider.GetService<EntityFramework.IRepositoryFactory<SampleDbContext>>());
            services.AddScoped<EntityFramework.IUnitOfWork<SampleDbContext>, EntityFramework.UnitOfWork<SampleDbContext>>();
            services.AddScoped<IUnitOfWork>(provider => provider.GetService<EntityFramework.IUnitOfWork<SampleDbContext>>());

            var serviceProvider = services.BuildServiceProvider();

            var dbContext = serviceProvider.GetRequiredService<SampleDbContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestMethod]
        public void Add_WithoutCallingComplete_DoesNotCommit()
        {
            // Arrange
            var repository = uow.Repository<Person>();

            // Act
            repository.Add(new Person
            {
                GivenName = "Hollie",
                FamilyName = "Marin"
            });

            var result = repository.Find(p => true);

            // Assert
            Assert.IsFalse(result.Any());
        }

        [TestMethod]
        public void Add_WithCompleteCall_Commits()
        {
            // Arrange
            var repository = uow.Repository<Person>();

            // Act
            repository.Add(new Person
            {
                GivenName = "Hollie",
                FamilyName = "Marin"
            });
            uow.Complete();

            var result = repository.Find(p => true);

            // Assert
            Assert.IsTrue(result.Any());
        }
    }
}
