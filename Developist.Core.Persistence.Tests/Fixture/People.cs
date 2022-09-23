namespace Developist.Core.Persistence.Tests.Fixture
{
    public static class People
    {
        public static IQueryable<Person> AsQueryable() => new[]
        {
            new Person(Guid.NewGuid()) { GivenName = "Dwayne", FamilyName = "Welsh", Age = 18 },
            new Person(Guid.NewGuid()) { GivenName = "Ed", FamilyName = "Stuart", Age = 24 },
            new Person(Guid.NewGuid()) { GivenName = "Hollie", FamilyName = "Marin", Age = 36 },
            new Person(Guid.NewGuid()) { GivenName = "Randall", FamilyName = "Bloom", Age = 55 },
            new Person(Guid.NewGuid()) { GivenName = "Glenn", FamilyName = "Hensley", Age = 27 },
            new Person(Guid.NewGuid()) { GivenName = "Phillipa", FamilyName = "Connor", Age = null },
            new Person(Guid.NewGuid()) { GivenName = "Peter", FamilyName = "Connor", Age = 12 },
            new Person(Guid.NewGuid()) { GivenName = "Ana", FamilyName = "Bryan", Age = 44 },
            new Person(Guid.NewGuid()) { GivenName = "Edgar", FamilyName = "Bernard", Age = 80 }
        }.AsQueryable();
    }
}
