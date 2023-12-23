namespace Developist.Core.Persistence.Tests.Fixture;

public class Book
{
    public Guid Id { get; init; }
    public string Title { get; init; } = default!;
    public ICollection<Person> Authors { get; } = new HashSet<Person>();
}
