using Developist.Core.Persistence.Entities;

namespace Developist.Core.Persistence.Tests.Fixture
{
    public class Book : EntityBase<Guid>
    {
        public Book() { }
        public Book(Guid id) : base(id) { }

        public string? Title { get; set; }
        public ICollection<Person> Authors { get; set; } = new HashSet<Person>();
        public Genre Genre { get; set; }
    }
}
