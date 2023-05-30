using Developist.Core.Persistence.Filtering;

namespace Developist.Core.Persistence.Tests.Fixture;

public class PersonByNameCriteria : IFilterCriteria<Person>
{
    public bool UsePartialMatching { get; set; }
    public string? GivenName { get; set; }
    public string? FamilyName { get; set; }

    public IQueryable<Person> Filter(IQueryable<Person> query)
    {
        if (GivenName is not null)
        {
            query = query.Where(p => p.GivenName != null && (UsePartialMatching ? p.GivenName.Contains(GivenName) : p.GivenName.Equals(GivenName)));
        }

        if (FamilyName is not null)
        {
            query = query.Where(p => p.FamilyName != null && (UsePartialMatching ? p.FamilyName.Contains(FamilyName) : p.FamilyName.Equals(FamilyName)));
        }

        return query;
    }
}
