namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public class SampleRepositoryFactory : RepositoryFactory<SampleDbContext>
{
    public SampleRepositoryFactory(IServiceProvider serviceProvider)
        : base(serviceProvider) { }
}
