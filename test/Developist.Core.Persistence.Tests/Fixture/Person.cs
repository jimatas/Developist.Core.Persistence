namespace Developist.Core.Persistence.Tests.Fixture;

public class Person
{
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string FullName => $"{GivenName} {FamilyName}";
    public int? Age { get; set; }
    public ICollection<Person> Friends { get; } = new HashSet<Person>();
    public Book? FavoriteBook { get; set; }
}
