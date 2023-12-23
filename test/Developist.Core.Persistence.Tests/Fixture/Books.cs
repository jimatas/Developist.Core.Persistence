namespace Developist.Core.Persistence.Tests.Fixture;

public static class Books
{
    public static IEnumerable<Book> AsEnumerable() => new Book[]
    {
        new() { Title = "The Great Gatsby" },
        new() { Title = "To Kill a Mockingbird" },
        new() { Title = "A Princess of Mars" },
        new() { Title = "Against the Fall of Night" },
        new() { Title = "Treasure Island" },
        new() { Title = "Call of the Wild" }
    };

    public static IQueryable<Book> AsQueryable() => AsEnumerable().AsQueryable();
}
