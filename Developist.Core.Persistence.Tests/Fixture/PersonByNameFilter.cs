namespace Developist.Core.Persistence.Tests.Fixture
{
    public class PersonByNameFilter : IQueryableFilter<Person>
    {
        public bool UsePartialMatching { get; set; }
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }

        public IQueryable<Person> Filter(IQueryable<Person> query)
        {
            if (!string.IsNullOrEmpty(GivenName))
            {
                query = query.Where(p => p.GivenName != null && (UsePartialMatching ? p.GivenName.Contains(GivenName) : p.GivenName.Equals(GivenName)));
            }

            if (!string.IsNullOrEmpty(FamilyName))
            {
                query = query.Where(p => p.FamilyName != null && (UsePartialMatching ? p.FamilyName.Contains(FamilyName) : p.FamilyName.Equals(FamilyName)));
            }

            return query;
        }
    }
}
