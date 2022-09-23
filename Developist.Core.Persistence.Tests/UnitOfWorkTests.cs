using Developist.Core.Persistence.InMemory.DependencyInjection;
using Developist.Core.Persistence.Tests.Fixture;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class UnitOfWorkTests
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
        public async Task Repository_GivenValidGenericType_ReturnsRepository()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

            // Act
            var personRepository = unitOfWork.Repository<Person>();

            // Assert
            Assert.IsNotNull(personRepository);
        }

        [TestMethod]
        public async Task Repository_CalledTwiceForSameGenericType_ReturnsSameRepository()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

            // Act
            IRepository<Person> personRepository = unitOfWork.Repository<Person>();
            IRepository<Person> anotherPersonRepository = unitOfWork.Repository<Person>();

            // Assert
            Assert.AreEqual(personRepository, anotherPersonRepository);
        }

        [TestMethod]
        public async Task Repository_CalledTwiceForDifferentGenericTypes_ReturnsTwoRepositories()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

            // Act
            IRepository<Book> bookRepository = unitOfWork.Repository<Book>();
            IRepository<Person> personRepository = unitOfWork.Repository<Person>();

            // Assert
            Assert.AreNotEqual(bookRepository, personRepository);
        }

        [TestMethod]
        public async Task CompleteAsync_ByDefault_FiresCompletedEvent()
        {
            // Arrange
            await using var provider = ConfigureServiceProvider(services => services.AddUnitOfWork());
            IUnitOfWork unitOfWork = provider.GetRequiredService<IUnitOfWork>();

            bool isCompleted = false;
            unitOfWork.Completed += (sender, e) => isCompleted = true;

            // Act
            await unitOfWork.CompleteAsync();

            // Assert
            Assert.IsTrue(isCompleted);
        }

        private static ServiceProvider ConfigureServiceProvider(Action<IServiceCollection> configureServices)
        {
            IServiceCollection services = new ServiceCollection();
            configureServices(services);
            return services.BuildServiceProvider();
        }
    }
}
