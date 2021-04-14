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
            dbContext.Database.EnsureCreated();

            uow = serviceProvider.GetRequiredService<IUnitOfWork>();
        }

        [TestCleanup]
        public void CleanUp()
        {
            (uow as EntityFramework.IUnitOfWork<SampleDbContext>)?.DbContext.Database.EnsureDeleted();
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

        [TestMethod]
        public void Add_PersonWithMessages_AddsMessagesAsWell()
        {
            // Arrange
            var repository = uow.Repository<Person>();

            var hollie = new Person
            {
                GivenName = "Hollie",
                FamilyName = "Marin"
            };
            
            hollie.SentMessages.Add(new Message
            {
                Sender = hollie,
                Text = "Hello world!"
            });

            // Act
            repository.Add(hollie);
            uow.Complete();

            var result = repository.Find(new PersonByNameFilter { FamilyName = "Marin" }, EntityIncludePaths.ForEntity<Person>().Include(p => p.ReceivedMessages));
            hollie = result.Single();

            // Assert
            Assert.AreEqual(hollie.GivenName, "Hollie");
            Assert.IsTrue(hollie.SentMessages.Any());
            Assert.IsFalse(hollie.ReceivedMessages.Any());
        }

        [TestMethod]
        public void Add_GivenMessageWithRecipientsAndSender_AddsMessageRecipientsAndSender()
        {
            var hollie = new Person { GivenName = "Hollie", FamilyName = "Marin" };
            var randall = new Person { GivenName = "Randall", FamilyName = "Bloom" };
            var glen = new Person { GivenName = "Glen", FamilyName = "Hensley" };

            var message = new Message
            {
                Sender = hollie,
                Text = "Hello world!"
            };

            message.Recipients.Add(randall);
            message.Recipients.Add(glen);
            
            var messageRepository = uow.Repository<Message>();
            messageRepository.Add(message);
            uow.Complete();

            var personRepository = uow.Repository<Person>();

            hollie = personRepository.Find(p => p.FamilyName == "Marin", EntityIncludePaths.ForEntity<Person>().Include(p => p.SentMessages)).Single();
            Assert.IsTrue(hollie.SentMessages.Any());

            glen = personRepository.Find(p => p.FamilyName == "Hensley", EntityIncludePaths.ForEntity<Person>().Include(p => p.ReceivedMessages)).Single();
            Assert.IsTrue(glen.ReceivedMessages.Any());
        }
    }
}
