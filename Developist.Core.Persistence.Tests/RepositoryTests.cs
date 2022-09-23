using Developist.Core.Persistence.Entities.IncludePaths;
using Developist.Core.Persistence.InMemory.DependencyInjection;
using Developist.Core.Persistence.Pagination;
using Developist.Core.Persistence.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class RepositoryTests
    {
        [TestMethod]
        public async Task EnsureUnitOfWorkRegistered()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());

            // Act
            IUnitOfWork? unitOfWork = provider.GetService<IUnitOfWork>();

            // Assert
            Assert.IsNotNull(unitOfWork);
        }

        [TestMethod]
        public void Initialize_WithNullUnitOfWork_ThrowsArgumentNullException()
        {
            // Arrange
            IUnitOfWork? unitOfWork = null;

            // Act
            void action() => _ = new InMemory.Repository<Person>(unitOfWork!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(() => action());
        }

        [TestMethod]
        public async Task CountAsync_ByDefault_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            // Act
            var count = await repository.CountAsync();

            // Assert
            Assert.AreEqual(9, count);
        }

        [TestMethod]
        public async Task CountAsync_GivenNullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();
            IQueryableFilter<Person>? filter = null;

            // Act
            async Task action() => await repository.CountAsync(filter!);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [DataTestMethod]
        [DataRow("ll", true, 3)]
        [DataRow("Hollie", false, 1)]
        [DataRow("John", false, 0)]
        public async Task CountAsync_GivenFilter_ReturnsExpectedResult(string givenName, bool usePartialMatching, int expectedResult)
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByNameFilter
            {
                GivenName = givenName,
                UsePartialMatching = usePartialMatching
            };

            // Act
            var result = await repository.CountAsync(filter);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public async Task FindAsync_GivenNullFilter_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();
            IQueryableFilter<Person>? filter = null;

            // Act
            async Task action() => await repository.FindAsync(filter!);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNullFilterAndNonNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            IQueryableFilter<Person>? filter = null;
            IQueryablePaginator<Person>? paginator = new SortingPaginator<Person>();

            // Act
            async Task action() => await repository.FindAsync(filter!, paginator);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNonNullFilterAndNullPaginator_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            IQueryableFilter<Person> filter = new PersonByNameFilter();
            IQueryablePaginator<Person>? paginator = null;

            // Act
            async Task action() => await repository.FindAsync(filter, paginator!);

            // Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task FindAsync_GivenNullIncludePaths_DoesNotThrow()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            IQueryableFilter<Person> filter = new PersonByNameFilter();
            IQueryablePaginator<Person> paginator = new SortingPaginator<Person>();
            IIncludePathsBuilder<Person>? includePaths = null;

            // Act
            await repository.FindAsync(filter, includePaths!);
            await repository.FindAsync(filter, paginator, includePaths!);

            // Assert
        }

        [TestMethod]
        public async Task FindAsync_GivenFilter_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByNameFilter { FamilyName = "Bloom" };

            // Act
            var people = await repository.FindAsync(filter);

            // Assert
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual("Randall", people.Single().GivenName);
        }

        [TestMethod]
        public async Task FindAsync_GivenFilterAndPaginator_ReturnsExpectedResult()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);

            IQueryableFilter<Person> filter = new PersonByNameFilter { FamilyName = "Bloom" };
            IQueryablePaginator<Person> paginator = new SortingPaginator<Person>().StartingAtPage(1).WithPageSizeOf(1).SortedByProperty(nameof(Person.FamilyName));

            // Act
            var people = await repository.FindAsync(filter, paginator);

            // Assert
            Assert.AreEqual(1, people.Count);
            Assert.AreEqual("Randall", people.Single().GivenName);
        }

        [TestMethod]
        public async Task Add_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();

            Person? entity = null;

            // Act
            void action() => repository.Add(entity!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task Add_WithoutCompleteAsync_DoesNotCommit()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = (InMemory.Repository<Person>)unitOfWork.Repository<Person>();

            var entity = new Person(Guid.NewGuid()) { GivenName = "John", FamilyName = "Smith" };

            // Act
            var countBeforeAdding = repository.DataStore.Count;
            repository.Add(entity);
            var countAfterAdding = repository.DataStore.Count;

            // Assert
            Assert.AreEqual(0, countBeforeAdding);
            Assert.AreEqual(countBeforeAdding, countAfterAdding);
        }

        [TestMethod]
        public async Task Add_WithCompleteAsync_Commits()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = (InMemory.Repository<Person>)unitOfWork.Repository<Person>();

            var entity = new Person(Guid.NewGuid()) { GivenName = "John", FamilyName = "Smith" };

            // Act
            var countBeforeAdding = repository.DataStore.Count;
            repository.Add(entity);
            await unitOfWork.CompleteAsync();
            var countAfterAdding = repository.DataStore.Count;

            // Assert
            Assert.AreNotEqual(countBeforeAdding, countAfterAdding);
        }

        [TestMethod]
        public async Task Remove_GivenNull_ThrowsArgumentNullException()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = unitOfWork.Repository<Person>();
            Person? entity = null;

            // Act
            void action() => repository.Remove(entity!);

            // Assert
            Assert.ThrowsException<ArgumentNullException>(action);
        }

        [TestMethod]
        public async Task Remove_WithoutCompleteAsync_DoesNotCommit()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = (InMemory.Repository<Person>)unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);
            var entity = (await repository.FindAsync(new PersonByNameFilter { FamilyName = "Hensley" })).Single();

            // Act
            var countBeforeRemoval = repository.DataStore.Count;
            repository.Remove(entity);
            var countAfterRemoval = repository.DataStore.Count;

            // Assert
            Assert.AreEqual(9, countBeforeRemoval);
            Assert.AreEqual(countBeforeRemoval, countAfterRemoval);
        }

        [TestMethod]
        public async Task Remove_WithCompleteAsync_Commits()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            var unitOfWork = provider.GetRequiredService<IUnitOfWork>();
            var repository = (InMemory.Repository<Person>)unitOfWork.Repository<Person>();

            await SeedRepositoryWithData(repository);
            var entity = (await repository.FindAsync(new PersonByNameFilter { FamilyName = "Hensley" })).Single();

            // Act
            var countBeforeRemoval = repository.DataStore.Count;
            repository.Remove(entity);
            await unitOfWork.CompleteAsync();
            var countAfterRemoval = repository.DataStore.Count;

            // Assert
            Assert.AreNotEqual(countBeforeRemoval, countAfterRemoval);
        }

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            IServiceCollection services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }

        private static async Task SeedRepositoryWithData(IRepository<Person> repository)
        {
            foreach (var person in People.AsQueryable())
            {
                repository.Add(person);
            }
            await repository.UnitOfWork.CompleteAsync();
        }
    }
}
