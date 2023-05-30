namespace Developist.Core.Persistence.IntegrationTests.Fixture;

public class Person
{
    public Person(Guid id = default) => Id = id;
    public Guid Id { get; }
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string FullName => $"{GivenName} {FamilyName}";
    public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
    public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();

}
