namespace Developist.Core.Persistence.Tests.Fixture;

public class Book
{
    public string Title { get; set; } = string.Empty;
    public ICollection<Person> Authors { get; } = new HashSet<Person>();
    public Genre Genre { get; set; }
}
