namespace Developist.Core.Persistence.EntityFrameworkCore.Tests.Fixture;

public class QueryExtenderSpy<T> : IQueryExtender<T>
{
    public IQueryable<T>? Query { get; private set; }

    public IQueryable<T> Extend(IQueryable<T> query)
    {
        return Query = query;
    }
}
