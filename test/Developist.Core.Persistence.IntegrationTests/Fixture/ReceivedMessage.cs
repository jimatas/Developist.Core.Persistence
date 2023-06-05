namespace Developist.Core.Persistence.IntegrationTests.Fixture;

public class ReceivedMessage
{
    public Person Recipient { get; set; } = default!;
    public Message Message { get; set; } = default!;
}
