using Developist.Core.Persistence.Entities;

namespace Developist.Core.Persistence.Tests.Fixture
{
    public class Message : EntityBase<Guid>
    {
        public Message() { }
        public Message(Guid id) : base(id) { }

        public string? Text { get; set; }

        public Person? Sender { get; set; }
        public ICollection<Person> Recipients { get; set; } = new HashSet<Person>();
    }
}
