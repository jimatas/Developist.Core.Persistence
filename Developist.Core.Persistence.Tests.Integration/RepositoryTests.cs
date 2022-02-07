// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Integration.Fixture;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Linq;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests.Integration
{
    [TestClass]
    public class RepositoryTests
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

            var result = repository.Find(new PersonByNameFilter { FamilyName = "Marin" }, new IncludePathCollection<Person>().Include(p => p.ReceivedMessages));
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

            hollie = personRepository.Find(p => p.FamilyName == "Marin", includePaths => includePaths.Include(p => p.SentMessages)).Single();
            Assert.IsTrue(hollie.SentMessages.Any());

            glen = personRepository.Find(p => p.FamilyName == "Hensley", includePaths => includePaths.Include(p => p.ReceivedMessages)).Single();
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

            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 2).SortedBy(nameof(Person.FamilyName));

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
        public async Task AllAsync_UsingPaginator_ReturnsExpectedResult()
        {
            // Arrange
            var people = new[]
            {
                new Person { GivenName = "Dwayne", FamilyName = "Welsh" },
                new Person { GivenName = "Ed", FamilyName = "Stuart" },
                new Person { GivenName = "Hollie", FamilyName = "Marin" },
                new Person { GivenName = "Randall", FamilyName = "Bloom" },
            };

            uow.People().AddRange(people);
            await uow.CompleteAsync();

            var paginator = new SortingPaginator<Person>().StartingAt(1).WithPageSizeOf(2).SortedBy("Id", SortDirection.Descending);

            // Act
            var paginatedResult = await uow.People().AllAsync(paginator);

            // Assert
            Assert.AreEqual(2, paginatedResult.Count());
            Assert.AreEqual("Randall", paginatedResult[0].GivenName);
            Assert.AreEqual("Hollie", paginatedResult[1].GivenName);
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
            var filteredResult = uow.People().Count(new PersonByNameFilter(/*andAlso: true*/)
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
        public async Task Add_WithExplicitTransaction_Commits()
        {
            await uow.BeginTransactionAsync();

            uow.People().Add(new()
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });

            var result = await uow.People().FindAsync(new PersonByNameFilter
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });

            Assert.IsFalse(result.Any());

            await uow.CompleteAsync();

            result = await uow.People().FindAsync(new PersonByNameFilter
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });

            Assert.IsTrue(result.Any());
        }

        [TestMethod]
        public async Task AllAsync_ByDefault_ReturnsPaginatedList()
        {
            // Arrange
            await uow.BeginTransactionAsync();

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

            await uow.CompleteAsync();

            // Act
            var paginator = new SortingPaginator<Person>(pageNumber: 1, pageSize: 5);

            var result = await uow.People().AllAsync(paginator);

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(paginator.PageNumber, result.PageNumber);
            Assert.AreEqual(paginator.PageCount, result.PageCount);
        }
    }
}
