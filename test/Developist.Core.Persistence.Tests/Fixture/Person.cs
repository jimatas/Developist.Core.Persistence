namespace Developist.Core.Persistence.Tests.Fixture;

public class Person
{
    public Guid Id { get; init ; }
    public string GivenName { get; init; } = default!;
    public string FamilyName { get; init; } = default!;
    public int? Age { get; init; }
    public Book? FavoriteBook { get; set; }

    public override string ToString()
    {
        return $"{GivenName} {FamilyName}";
    }
}
