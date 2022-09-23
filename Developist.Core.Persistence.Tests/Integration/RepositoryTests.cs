using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.EntityFrameworkCore.DependencyInjection;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests.Integration
{
    [TestClass]
    public class RepositoryTests
    {
        private const string ConnectionString = $"Server=(localdb)\\mssqllocaldb;Database=Developist_Core_Persistence_IntegrationTests;Trusted_Connection=true;MultipleActiveResultSets=true";

        [TestMethod]
        public async Task Add_WithoutCallingCompleteAsync_DoesNotCommit()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            // Act
            repository.Add(new Person
            {
                GivenName = "John",
                FamilyName = "Smith"
            });

            var people = await repository.AllAsync(p => p.StartingAtPage(1).WithPageSizeOf(int.MaxValue).SortedByProperty(p => p.FamilyName));

            // Assert
            Assert.IsFalse(people.Any());
        }

        [TestMethod]
        public async Task Add_WithCompleteAsync_Commits()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            // Act
            repository.Add(new Person
            {
                GivenName = "John",
                FamilyName = "Smith"
            });
            await unitOfWork.CompleteAsync();

            var people = await repository.AllAsync(p => p.StartingAtPage(1).WithPageSizeOf(int.MaxValue).SortedByProperty(p => p.FamilyName));

            // Assert
            Assert.IsTrue(people.Any());
        }

        [TestMethod]
        public async Task Add_PersonWithMessages_AddsMessagesAsWell()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            var hollie = new Person
            {
                GivenName = "Hollie",
                FamilyName = "Marin"
            };

            hollie.SentMessages.Add(new Message
            {
                Sender = hollie,
                Text = "Hello, world!"
            });

            // Act
            repository.Add(hollie);
            await unitOfWork.CompleteAsync();

            var result = await repository.FindAsync(p => p.FamilyName == "Marin", includePaths => includePaths.Include(p => p.ReceivedMessages));
            hollie = result.Single();

            // Assert
            Assert.AreEqual(hollie.GivenName, "Hollie");
            Assert.IsTrue(hollie.SentMessages.Any());
            Assert.IsFalse(hollie.ReceivedMessages.Any());
        }

        [TestMethod]
        public async Task Add_GivenMessageWithRecipientsAndSender_AddsMessageRecipientsAndSender()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var hollie = new Person { GivenName = "Hollie", FamilyName = "Marin" };
            var randall = new Person { GivenName = "Randall", FamilyName = "Bloom" };
            var glen = new Person { GivenName = "Glen", FamilyName = "Hensley" };

            var message = new Message
            {
                Sender = hollie,
                Text = "Hello, world!"
            };

            message.Recipients.Add(randall);
            message.Recipients.Add(glen);

            var messageRepository = unitOfWork.Repository<Message>();

            // Act
            messageRepository.Add(message);
            await unitOfWork.CompleteAsync();

            // Assert
            var personRepository = unitOfWork.Repository<Person>();

            hollie = (await personRepository.FindAsync(p => p.FamilyName == "Marin", includePaths => includePaths.Include(p => p.SentMessages))).Single();
            Assert.IsTrue(hollie.SentMessages.Any());

            glen = (await personRepository.FindAsync(p => p.FamilyName == "Hensley", includePaths => includePaths.Include(p => p.ReceivedMessages))).Single();
            Assert.IsTrue(glen.ReceivedMessages.Any());
        }

        [TestMethod]
        public async Task FindAsync_UsingPaginator_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(2).SortedByProperty(p => p.FamilyName);

            // Act
            var result = await repository.FindAsync(p => p.GivenName!.Contains("ll")); // 3
            var paginatedResult = await repository.FindAsync(p => p.GivenName!.Contains("ll"), paginator); // 2

            // Assert
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual(2, paginatedResult.Count);
            Assert.AreEqual("Randall", paginatedResult[0].GivenName);
            Assert.AreEqual("Phillipa", paginatedResult[1].GivenName);
        }

        [TestMethod]
        public async Task AllAsync_UsingPaginator_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(2).SortedByProperty(p => p.FamilyName, SortDirection.Descending);

            // Act
            var paginatedResult = await repository.AllAsync(paginator);

            // Assert
            Assert.AreEqual(2, paginatedResult.Count);
            Assert.AreEqual("Welsh", paginatedResult[0].FamilyName);
            Assert.AreEqual("Stuart", paginatedResult[1].FamilyName);
        }

        [TestMethod]
        public async Task CountAsync_ByDefaultAndFiltered_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            var totalPeople = await SeedRepositoryWithData(repository);

            // Act
            var unfilteredResult = await repository.CountAsync();
            var filteredResult = await repository.CountAsync(new PersonByNameFilter
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });
            var filteredByPredicateResult = await repository.CountAsync(p => p.GivenName!.Equals("Ed") || p.GivenName!.Equals("Glenn"));

            // Assert
            Assert.AreEqual(totalPeople, unfilteredResult);
            Assert.AreEqual(1, filteredResult);
            Assert.AreEqual(2, filteredByPredicateResult);
        }

        [TestMethod]
        public async Task Add_WithExplicitTransaction_Commits()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            await unitOfWork.BeginTransactionAsync();

            var repository = unitOfWork.Repository<Person>();

            // Act
            repository.Add(new Person
            {
                GivenName = "Glenn",
                FamilyName = "Hensley"
            });

            bool wasTransactional = unitOfWork.IsTransactional;
            await unitOfWork.CompleteAsync();

            // Assert
            Assert.IsTrue((await repository.FindAsync(new PersonByNameFilter { GivenName = "Glenn", FamilyName = "Hensley" })).Any());
            Assert.IsTrue(wasTransactional);
            Assert.IsFalse(unitOfWork.IsTransactional);
        }

        [TestMethod]
        public async Task AllAsync_ByDefault_ReturnsPaginatedList()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services
                => services.AddDbContext<SampleDbContext>(
                    dbContext => dbContext.UseSqlServer(ConnectionString),
                    ServiceLifetime.Scoped)
                .AddUnitOfWork<SampleDbContext>());

            await using var unitOfWork = new UnitOfWorkWrapper<SampleDbContext>(provider.GetRequiredService<IUnitOfWork<SampleDbContext>>(), async uow => await uow.DbContext.Database.EnsureDeletedAsync());
            await unitOfWork.DbContext.Database.EnsureCreatedAsync();

            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            var paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(5).SortedByProperty(p => p.FamilyName);

            // Act
            var result = await repository.AllAsync(paginator);

            // Assert
            Assert.AreEqual(5, result.Count);
            Assert.AreEqual(paginator.PageNumber, result.PageNumber);
            Assert.AreEqual(paginator.PageCount, result.PageCount);
        }

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            IServiceCollection services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }

        private static async Task<int> SeedRepositoryWithData(IRepository<Person> repository)
        {
            var count = 0;
            foreach (var person in People.AsQueryable())
            {
                repository.Add(person);
                count++;
            }
            await repository.UnitOfWork.CompleteAsync();
            return count;
        }
    }
}
