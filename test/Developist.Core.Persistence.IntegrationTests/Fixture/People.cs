namespace Developist.Core.Persistence.IntegrationTests.Fixture;

public static class People
{
    public static IQueryable<Person> AsQueryable() => new Person[]
    {
        new(Guid.NewGuid()) { GivenName = "Dwayne", FamilyName = "Welsh" },
        new(Guid.NewGuid()) { GivenName = "Ed", FamilyName = "Stuart" },
        new(Guid.NewGuid()) { GivenName = "Hollie", FamilyName = "Marin" },
        new(Guid.NewGuid()) { GivenName = "Randall", FamilyName = "Bloom" },
        new(Guid.NewGuid()) { GivenName = "Glenn", FamilyName = "Hensley" },
        new(Guid.NewGuid()) { GivenName = "Phillipa", FamilyName = "Connor" },
        new(Guid.NewGuid()) { GivenName = "Peter", FamilyName = "Connor" },
        new(Guid.NewGuid()) { GivenName = "Ana", FamilyName = "Bryan" },
        new(Guid.NewGuid()) { GivenName = "Edgar", FamilyName = "Bernard" }
    }.AsQueryable();

    public static Task SeedWithDataAsync(this IRepository<Person> repository, CancellationToken cancellationToken = default)
    {
        foreach (var person in AsQueryable())
        {
            repository.Add(person);
        }

        return repository.UnitOfWork.CompleteAsync(cancellationToken);
    }
}
