using Developist.Core.Persistence.Tests.Fixture;

namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public static class RepositoryExtensions
{
    public static async Task<IRepository<Person>> InitializedWithPeopleAsync(this IRepository<Person> personRepository)
    {
        foreach (var person in People.AsEnumerable())
        {
            personRepository.Add(person);
        }
        await personRepository.UnitOfWork.CompleteAsync();

        return personRepository;
    }
}
