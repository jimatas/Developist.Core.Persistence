using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public class SocialPerson : Person
{
    public ICollection<SocialPerson> Friends { get; } = new HashSet<SocialPerson>();
}
