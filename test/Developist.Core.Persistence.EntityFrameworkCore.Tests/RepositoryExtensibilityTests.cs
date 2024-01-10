using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class RepositoryExtensibilityTests : TestClassBase
{
    [TestMethod]
    public void ExtendWithFeature_GivenNullQueryExtender_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IQueryExtender<Person> queryExtender = null!;

        // Act
        void action() => repository.ExtendWithFeature(queryExtender, "null feature");

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(queryExtender), exception.ParamName);
    }

    [TestMethod]
    public void ExtendWithFeature_GivenNullFeatureName_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IQueryExtender<Person> queryExtender = new Mock<IQueryExtender<Person>>().Object;
        string featureName = null!;

        // Act
        void action() => repository.ExtendWithFeature(queryExtender, featureName);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(featureName), exception.ParamName);
    }

    [DataTestMethod]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\r\n\t")]
    public void ExtendWithFeature_GivenEmptyOrWhiteSpaceStringForFeatureName_ThrowsArgumentException(string featureName)
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IQueryExtender<Person> queryExtender = new Mock<IQueryExtender<Person>>().Object;

        // Act
        void action() => repository.ExtendWithFeature(queryExtender, featureName);

        // Assert
        var exception = Assert.ThrowsException<ArgumentException>(action);
        Assert.AreEqual(nameof(featureName), exception.ParamName);
    }

    [TestMethod]
    public void WithoutTracking_CalledOnRepository_ReturnsNoTrackingRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.WithoutTracking();

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);
        
        var nonTrackingRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.NoTrackingQueryExtender<Person>>(nonTrackingRepository.QueryExtender);
    }

    [TestMethod]
    public void WithTracking_CalledOnRepository_ReturnsTrackingRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.WithTracking();

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);

        var trackingRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.TrackingQueryExtender<Person>>(trackingRepository.QueryExtender);
    }

    [TestMethod]
    public void BuildIncludes_ByDefault_ReturnsIncludesBuilder()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        var includes = repository.BuildIncludes();

        // Assert
        Assert.IsNotNull(includes);
    }

    [TestMethod]
    public void WithIncludes_GivenNullIncludes_ThrowsArgumentNullException()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();
        IIncludesBuilder<Person> includes = null!;

        // Act
        void action() => repository.WithIncludes(includes);

        // Assert
        var exception = Assert.ThrowsException<ArgumentNullException>(action);
        Assert.AreEqual(nameof(includes), exception.ParamName);
    }

    [TestMethod]
    public void WithIncludes_CalledOnRepository_ReturnsIncludableRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.WithIncludes(includes => includes.Include(nameof(Person.FavoriteBook)));

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);

        var includableRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.IncludableQueryExtender<Person>>(includableRepository.QueryExtender);
    }

    [TestMethod]
    public void WithSplitQueries_CalledOnRepository_ReturnsQuerySplittingRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.WithSplitQueries();

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);

        var querySplittingRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.SplitQueryExtender<Person>>(querySplittingRepository.QueryExtender);
    }

    [TestMethod]
    public void WithoutSplitQueries_CalledOnRepository_ReturnsNonQuerySplittingRepository()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.WithoutSplitQueries();

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);

        var nonQuerySplittingRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.SingleQueryExtender<Person>>(nonQuerySplittingRepository.QueryExtender);
    }

    [TestMethod]
    public void DecoratingRepository_WithMultipleQueryExtenders_ReturnsRepositoryWithCompositeQueryExtender()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        // Act
        repository = repository.ExtendWithFeature(new Mock<IQueryExtender<Person>>().Object, "mocked feature 1")
            .ExtendWithFeature(new Mock<IQueryExtender<Person>>().Object, "mocked feature 2")
            .ExtendWithFeature(new Mock<IQueryExtender<Person>>().Object, "mocked feature 3");

        // Assert
        Assert.IsInstanceOfType<ExtendableQueryRepository<Person>>(repository);

        var extendedRepository = (ExtendableQueryRepository<Person>)repository;
        Assert.IsInstanceOfType<RepositoryExtensions.CompositeQueryExtender<Person>>(extendedRepository.QueryExtender);
    }

    [TestMethod]
    public void ExtendWithFeature_CalledOnUnsupportedRepository_ThrowsNotSupportedException()
    {
        // Arrange
        var repository = new Mock<IRepository<Person>>().Object;
        var queryExtender = new Mock<IQueryExtender<Person>>().Object;

        // Act
        void action() => repository.ExtendWithFeature(queryExtender, "mocked features");

        // Assert
        var exception = Assert.ThrowsException<NotSupportedException>(action);
        Assert.IsTrue(Regex.IsMatch(exception.Message, "The repository of type '.*' does not have support for mocked features\\."));
    }
}
