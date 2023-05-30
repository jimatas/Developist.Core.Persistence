namespace Developist.Core.Persistence.Tests.Fixture;

public static class People
{
    public static IQueryable<Person> AsQueryable() => new Person[]
    {
        new() { GivenName = "Dwayne", FamilyName = "Welsh", Age = 18 },
        new() { GivenName = "Ed", FamilyName = "Stuart", Age = 24 },
        new() { GivenName = "Hollie", FamilyName = "Marin", Age = 36 },
        new() { GivenName = "Randall", FamilyName = "Bloom", Age = 55 },
        new() { GivenName = "Glenn", FamilyName = "Hensley", Age = 27 },
        new() { GivenName = "Phillipa", FamilyName = "Connor", Age = null },
        new() { GivenName = "Peter", FamilyName = "Connor", Age = 12 },
        new() { GivenName = "Ana", FamilyName = "Bryan", Age = 44 },
        new() { GivenName = "Edgar", FamilyName = "Bernard", Age = 80 }
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
