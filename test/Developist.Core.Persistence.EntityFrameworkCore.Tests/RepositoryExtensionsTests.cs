using Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;
using Developist.Core.Persistence.Tests.Fixture;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests;

[TestClass]
public class RepositoryExtensionsTests : TestClassBase
{
    [TestMethod]
    public async Task AnyAsync_GivenPredicate_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.AnyAsync(p => p.FamilyName.Equals("Marin"));

        // Assert
        Assert.IsTrue(result);
    }

    [TestMethod]
    public async Task CountAsync_GivenPredicate_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.CountAsync(p => p.Age > 45);

        // Assert
        Assert.AreEqual(2, result);
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_GivenPredicate_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.FirstOrDefaultAsync(p => p.Age > 45);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Randall Bloom", result.ToString());
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_GivenPredicate_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.SingleOrDefaultAsync(p => p.FamilyName.Equals("Connor") && p.Age != null);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("Peter Connor", result.ToString());
    }

    [TestMethod]
    public async Task ListAsync_GivenPaginationConfigurator_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.ListAsync(paginator => paginator.SetPageNumber(2).SetPageSize(1).SortBy(p => p.Age));

        // Assert
        Assert.AreEqual("Peter Connor", result.Single().ToString());
    }

    [TestMethod]
    public async Task ListAsync_GivenPredicateAndPaginationConfigurator_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        await repository.InitializedWithPeopleAsync();

        // Act
        var result = await repository.ListAsync(
            p => p.Age < 20,
            paginator => paginator.SetPageNumber(1).SetPageSize(1).SortBy(p => p.Age, SortDirection.Descending));

        // Assert
        Assert.AreEqual("Dwayne Welsh", result.Single().ToString());
    }

    [TestMethod]
    public async Task AnyAsync_CalledOnNoTrackingRepository_UsesNoTrackingQuery()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        var spyingExtender = new QueryExtenderSpy<Person>();

        // Act
        _ = await repository.WithoutTracking()
            .ExtendWithFeature(spyingExtender, "spying on features")
            .AnyAsync();

        // Assert
        Assert.IsNotNull(spyingExtender.Query);
        Assert.IsTrue(spyingExtender.Query.IsNoTracking());
    }

    [TestMethod]
    public async Task AnyAsync_CalledOnTrackingRepository_UsesTrackingQuery()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        var spyingExtender = new QueryExtenderSpy<Person>();

        // Act
        _ = await repository.WithTracking()
            .ExtendWithFeature(spyingExtender, "spying on features")
            .AnyAsync();

        // Assert
        Assert.IsNotNull(spyingExtender.Query);
        Assert.IsTrue(spyingExtender.Query.IsTracking());
    }

    [TestMethod]
    public async Task FirstOrDefaultAsync_CalledOnIncludableRepository_UsesIncludableQuery()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<Person>();

        var spyingExtender = new QueryExtenderSpy<Person>();

        // Act
        _ = await repository.WithIncludes(includes => includes.Include(nameof(Person.FavoriteBook)))
            .ExtendWithFeature(spyingExtender, "spying on features")
            .FirstOrDefaultAsync();

        // Assert
        Assert.IsNotNull(spyingExtender.Query);
        Assert.IsTrue(spyingExtender.Query.IsIncludable());
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_CalledOnQuerySplittingRepository_UsesSplitQuery()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<SocialPerson>();

        await unitOfWork.CompleteAsync();

        var spyingExtender = new QueryExtenderSpy<SocialPerson>();

        // Act
        _ = await repository.WithSplitQueries()
            .ExtendWithFeature(spyingExtender, "spying on features")
            .SingleOrDefaultAsync(p => p.GivenName.Equals("John") && p.FamilyName.Equals("Doe"));

        // Assert
        Assert.IsNotNull(spyingExtender.Query);
        Assert.IsTrue(spyingExtender.Query.IsSplitQuery());
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_CalledOnNonQuerySplittingRepository_UsesSingleQuery()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<SocialPerson>();

        await unitOfWork.CompleteAsync();

        var spyingExtender = new QueryExtenderSpy<SocialPerson>();

        // Act
        _ = await repository.WithoutSplitQueries()
            .ExtendWithFeature(spyingExtender, "spying on features")
            .SingleOrDefaultAsync(p => p.GivenName.Equals("John") && p.FamilyName.Equals("Doe"));

        // Assert
        Assert.IsNotNull(spyingExtender.Query);
        Assert.IsTrue(spyingExtender.Query.IsSingleQuery());
    }

    [TestMethod]
    public async Task SingleOrDefaultAsync_CalledOnRepositoryWithMultipleQueryExtenders_ReturnsExpectedResult()
    {
        // Arrange
        using var serviceProvider = ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());
        var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWorkBase>();
        var repository = unitOfWork.Repository<SocialPerson>();

        var person = new SocialPerson
        {
            GivenName = "John",
            FamilyName = "Doe",
            FavoriteBook = new Book { Title = "The Once and Future King" }
        };

        person.Friends.Add(new SocialPerson
        {
            GivenName = "Jane",
            FamilyName = "Doe",
            FavoriteBook = new Book { Title = "Dragons of Autumn Twilight" }
        });

        repository.Add(person);

        await unitOfWork.CompleteAsync();

        // Act
        var result = await repository
            .WithoutTracking()
            .WithIncludes(includes => includes.Include(p => p.FavoriteBook).Include(p => p.Friends).ThenInclude(p => p.FavoriteBook))
            .WithSplitQueries()
            .SingleOrDefaultAsync(p => p.FavoriteBook!.Title.Contains("King"));

        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotNull(result.FavoriteBook);
        Assert.IsTrue(result.Friends.Any());
        Assert.IsTrue(result.Friends.All(friend => friend.FavoriteBook is not null));
    }
}
