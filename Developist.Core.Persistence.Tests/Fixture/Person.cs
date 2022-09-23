using Developist.Core.Persistence.Entities;

namespace Developist.Core.Persistence.Tests.Fixture
{
    public class Person : EntityBase<Guid>
    {
        public Person() { }
        public Person(Guid id) : base(id) { }

        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public int? Age { get; set; }
        public ICollection<Person> Friends { get; set; } = new HashSet<Person>();
        public Book? FavoriteBook { get; set; }
        public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
    }
}
