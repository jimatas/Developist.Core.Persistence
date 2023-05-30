namespace Developist.Core.Persistence.IntegrationTests.Fixture;

public class Message
{
    public Message(Guid id = default) => Id = id;
    public Guid Id { get; }
    public string Text { get; set; } = string.Empty;
    public Person? Sender { get; set; }
    public ICollection<Person> Recipients { get; set; } = new HashSet<Person>();
}
