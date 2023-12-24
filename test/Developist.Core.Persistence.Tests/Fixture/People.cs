namespace Developist.Core.Persistence.Tests.Fixture;

public static class People
{
    public static IEnumerable<Person> AsEnumerable() => new Person[]
    {
        new() { GivenName = "Dwayne", FamilyName = "Welsh", Age = 18 },
        new() { GivenName = "Ed", FamilyName = "Stuart", Age = 24 },
        new() { GivenName = "Hollie", FamilyName = "Marin", Age = 36 },
        new() { GivenName = "Randall", FamilyName = "Bloom", Age = 55 },
        new() { GivenName = "Glenn", FamilyName = "Hensley", Age = 27 },
        new() { GivenName = "Phillipa", FamilyName = "Connor" },
        new() { GivenName = "Peter", FamilyName = "Connor", Age = 12 },
        new() { GivenName = "Ana", FamilyName = "Bryan", Age = 44 },
        new() { GivenName = "Edgar", FamilyName = "Bernard", Age = 80 }
    };

    public static IQueryable<Person> AsQueryable() => AsEnumerable().AsQueryable();
}
