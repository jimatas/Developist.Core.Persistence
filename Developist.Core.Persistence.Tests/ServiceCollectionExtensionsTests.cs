using Developist.Core.Persistence.InMemory;
using Developist.Core.Persistence.InMemory.DependencyInjection;

using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class ServiceCollectionExtensionsTests
    {
        private readonly IServiceCollection services = new ServiceCollection();

        [TestMethod]
        public async Task AddUnitOfWork_ByDefault_RegistersInMemoryUnitOfWork()
        {
            // Arrange

            // Act
            services.AddUnitOfWork();
            await using var serviceProvider = services.BuildServiceProvider();

            IUnitOfWork? unitOfWork = serviceProvider.GetService<IUnitOfWork>();

            // Assert
            Assert.IsNotNull(unitOfWork);
            Assert.IsInstanceOfType(unitOfWork, typeof(UnitOfWork));
        }

        [TestMethod]
        public void AddUnitOfWork_GivenIncorectRepositoryFactoryType_ThrowsArgumentException()
        {
            // Arrange
            var incorrectRepositoryFactoryType = typeof(object);

            // Act
            void action() => services.AddUnitOfWork(incorrectRepositoryFactoryType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddUnitOfWork_GivenRepositoryFactoryInterfaceType_ThrowsArgumentException()
        {
            // Arrange
            var repositoryFactoryInterfaceType = typeof(IRepositoryFactory);

            // Act
            void action() => services.AddUnitOfWork(repositoryFactoryInterfaceType);

            // Assert
            Assert.ThrowsException<ArgumentException>(action);
        }

        [TestMethod]
        public void AddUnitOfWork_GivenValidRepositoryFactoryType_DoesNotThrow()
        {
            // Arrange
            var validRepositoryFactoryType = typeof(RepositoryFactory);

            // Act
            // Assert
            services.AddUnitOfWork(validRepositoryFactoryType);
        }
    }
}
