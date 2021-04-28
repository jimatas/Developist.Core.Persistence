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
        private IUnitOfWorkManager uowManager;
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
            uowManager = serviceProvider.GetRequiredService<IUnitOfWorkManager>();
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

        [TestMethod]
        public void Find_UsingPaginator_ReturnsExpectedResult()
        {
            // Arrange
            var people = new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh" },
                new Person { GivenName = "Ed", FamilyName = "Stuart" },
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
                new Person { GivenName = "Glenn", FamilyName = "Hensley" },
                new Person { GivenName = "Phillipa", FamilyName = "Connor" },
                new Person { GivenName = "Ana", FamilyName = "Bryan" },
                new Person { GivenName = "Edgar", FamilyName = "Bernard" }
            };

            uow.People().AddRange(people);
            uow.Complete();

            IQueryablePaginator<Person> paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2).SortedBy(nameof(Person.FamilyName));

            // Act
            var result = uow.People().Find(p => p.GivenName.Contains("ll")); // 3
            var paginatedResult = uow.People().Find(p => p.GivenName.Contains("ll"), paginator); // 2

            // Assert
            Assert.AreEqual(3, result.Count());
            Assert.AreEqual(2, paginatedResult.Count());
            Assert.AreEqual("Randall", paginatedResult.First().GivenName);
            Assert.AreEqual("Phillipa", paginatedResult.Last().GivenName);
        }

        [TestMethod]
        public void Count_ByDefaultAndGivenFilter_ReturnsExpectedResult()
        {
            // Arrange
            var people = new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh" },
                new Person { GivenName = "Ed", FamilyName = "Stuart" },
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
                new Person { GivenName = "Glenn", FamilyName = "Hensley" },
                new Person { GivenName = "Phillipa", FamilyName = "Connor" },
                new Person { GivenName = "Ana", FamilyName = "Bryan" },
                new Person { GivenName = "Edgar", FamilyName = "Bernard" }
            };

            uow.People().AddRange(people);
            uow.Complete();

            // Act
            var unfilteredResult = uow.People().Count();
            var filteredResult = uow.People().Count(new PersonByNameFilter(andAlso: true)
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });
            var filteredByPredicateResult = uow.People().Count(predicate: p => p.GivenName.Equals("Ed") || p.GivenName.Equals("Glenn"));

            // Assert
            Assert.AreEqual(people.Length, unfilteredResult);
            Assert.AreEqual(1, filteredResult);
            Assert.AreEqual(2, filteredByPredicateResult);
        }

        [TestMethod]
        public void StartNew_MultipleCallsWhileRegisteredAsTransient_ReturnsDistinctInstances()
        {
            using var uow1 = uowManager.StartNew();
            using var uow2 = uowManager.StartNew();

            Assert.AreNotEqual(uow1, uow2);

            uow1.People().Add(new()
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });

            var result = uow2.People().Find(new PersonByNameFilter { FamilyName = "Hensley" });
            Assert.IsFalse(result.Any());

            uow1.Complete();
            result = uow2.People().Find(new PersonByNameFilter { FamilyName = "Hensley" });

            Assert.IsTrue(result.Any());
        }
    }
}
