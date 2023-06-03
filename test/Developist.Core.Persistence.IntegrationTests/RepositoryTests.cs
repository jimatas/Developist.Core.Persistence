using Developist.Core.Persistence.EntityFrameworkCore;
using Developist.Core.Persistence.IntegrationTests.Fixture;
using Developist.Core.Persistence.IntegrationTests.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.IntegrationTests;

[TestClass]
public class RepositoryTests
{
    [TestMethod]
    public async Task Add_WithoutCallingCompleteAsync_DoesNotCommit()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();

        // Act
        unitOfWork.Repository<Person>().Add(new()
        {
            GivenName = "John",
            FamilyName = "Smith"
        });

        var result = await unitOfWork.Repository<Person>()
            .ListAsync(paginator => paginator.StartingAtPage(1).WithPageSize(int.MaxValue).SortedByProperty(p => p.FamilyName));

        // Assert
        Assert.IsFalse(result.Any());
    }

    [TestMethod]
    public async Task Add_WithCompleteAsync_Commits()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();

        // Act
        unitOfWork.Repository<Person>().Add(new()
        {
            GivenName = "John",
            FamilyName = "Smith"
        });

        await unitOfWork.CompleteAsync();

        var result = await unitOfWork.Repository<Person>()
            .ListAsync(paginator => paginator.StartingAtPage(1).WithPageSize(int.MaxValue).SortedByProperty(p => p.FamilyName));

        // Assert
        Assert.IsTrue(result.Any());
    }

    [TestMethod]
    public async Task Add_GivenPersonWithMessages_AddsMessagesAsWell()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();

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
        unitOfWork.Repository<Person>().Add(hollie);
        await unitOfWork.CompleteAsync();

        var result = await unitOfWork.Repository<Person>()
            .WithIncludes(related => related.Include(person => person.ReceivedMessages))
            .ListAsync(
                person => person.GivenName == "Hollie" && person.FamilyName == "Marin",
                paginator => paginator.StartingAtPage(1).WithPageSize(1).SortedByProperty(p => p.FamilyName));

        hollie = result.Single();

        // Assert
        Assert.IsTrue(hollie.SentMessages.Any());
        Assert.IsFalse(hollie.ReceivedMessages.Any());
    }

    [TestMethod]
    public async Task Add_GivenMessageWithRecipientsAndSender_AddsMessageRecipientsAndSender()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

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

        // Act
        unitOfWork.Repository<Message>().Add(message);
        await unitOfWork.CompleteAsync();

        // Assert
        hollie = await unitOfWork.Repository<Person>()
            .WithIncludes(related => related.Include(person => person.SentMessages))
            .SingleOrDefaultAsync(person => person.GivenName == "Hollie" && person.FamilyName == "Marin");

        Assert.IsTrue(hollie.SentMessages.Any());

        glen = await unitOfWork.Repository<Person>()
            .WithIncludes(related => related.Include(person => person.SentMessages))
            .SingleOrDefaultAsync(person => person.GivenName == "Glen" && person.FamilyName == "Hensley");

        Assert.IsTrue(glen.ReceivedMessages.Any());
    }

    [TestMethod]
    public async Task Add_WithExplicitTransaction_Commits()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();
        await unitOfWork.BeginTransactionAsync();

        // Act
        unitOfWork.Repository<Person>().Add(new()
        {
            GivenName = "Glenn",
            FamilyName = "Hensley"
        });

        bool hadActiveTransaction = unitOfWork.HasActiveTransaction;
        await unitOfWork.CompleteAsync();

        // Assert
        Assert.AreEqual(1, await unitOfWork.Repository<Person>().CountAsync());
        Assert.IsTrue(hadActiveTransaction);
        Assert.IsFalse(unitOfWork.HasActiveTransaction);
    }

    [TestMethod]
    public async Task Remove_ByDefault_RemovesEntity()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();
        await unitOfWork.BeginTransactionAsync();
        await unitOfWork.Repository<Person>().SeedWithDataAsync();
        await unitOfWork.CompleteAsync();

        // Act
        var countBeforeRemoval = await unitOfWork.Repository<Person>().CountAsync();

        var peter = await unitOfWork.Repository<Person>()
            .SingleOrDefaultAsync(person => person.GivenName == "Peter" && person.FamilyName == "Connor");

        unitOfWork.Repository<Person>().Remove(peter);
        await unitOfWork.CompleteAsync();

        var countAfterRemoval = await unitOfWork.Repository<Person>().CountAsync();

        // Assert
        Assert.IsTrue(countBeforeRemoval - countAfterRemoval == 1);
    }

    [TestMethod]
    public async Task CountAsync_GivenFilterCriteria_ReturnsExpectedResult()
    {
        // Arrange
        await using var serviceProvider = ServiceProviderHelper.ConfigureServiceProvider(services => services.AddUnitOfWork<SampleDbContext>());

        await using var unitOfWork = serviceProvider.GetRequiredService<IUnitOfWork<SampleDbContext>>()
            .AsDisposableUnitOfWork(async uow => await uow.DbContext.Database.EnsureDeletedAsync());

        await unitOfWork.DbContext.Database.EnsureCreatedAsync();
        await unitOfWork.BeginTransactionAsync();

        await unitOfWork.Repository<Person>().SeedWithDataAsync();
        await unitOfWork.CompleteAsync();

        // Act
        var count = await unitOfWork.Repository<Person>().CountAsync();
        var filteredCount = await unitOfWork.Repository<Person>().CountAsync(p => p.GivenName == "Glenn" || p.GivenName == "Ed");

        // Assert
        Assert.AreEqual(People.AsQueryable().Count(), count);
        Assert.AreEqual(2, filteredCount);
    }
}
