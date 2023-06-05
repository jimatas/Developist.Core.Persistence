# Developist.Core.Persistence

### Entity Framework Core and in-memory Unit of Work and Repository implementations

If you are using the Entity Framework Core version of the package, define your `DbContext` subclass and any entity type configurations that are needed to map your entities to the database schema.

Register the [`IUnitOfWork`](Developist.Core.Persistence/IUnitOfWork.cs) depdendency with the built-in dependency injection framework by using the appropriate DI-registration extension method: either `AddUnitOfWork<TContext>()` for the Entity Framework Core version, or `AddUnitOfWork()` for the in-memory version of the package.

There are several overloads of this extension method provided that accept an [`IRepositoryFactory`](Developist.Core.Persistence/IRepositoryFactory.cs) type or instance, which will be used instead of the default factory to create the [`IRepository<T>`](Developist.Core.Persistence/IRepository`1.cs) instances returned by the unit of work's `Repository<T>()` method.

#### Usage

A typical usage scenario involves injecting the `IUnitOfWork` interface through the constructor of a consumer. You can subsequently query entities and persist them using the repositories obtained through the unit of work's `Repository<T>()` method, such as in the following example.

```csharp
public PersonFinderService(IUnitOfWork uow) => _uow = uow;

public Task<IPaginatedList<Person>> FindAllByNameAsync(string familyName, string? givenName = default, int? pageNumber = 1)
{
    IFilterCriteria<Person> criteria = new PersonByNameFilterCriteria(familyName, givenName);
    IPaginator paginator = new SortingPaginator(pageNumber, pageSize: 10)
        .SortedByProperty(p => p.FamilyName)
        .SortedByProperty(p => p.GivenName);
    
    return await _uow.Repository<Person>().ListAsync(criteria, paginator);
}
```

The [`IFilterCriteria<T>`](Developist.Core.Persistence/Filtering/IFilterCriteria`1.cs) interface in the previous example is an implementation of the Specification pattern. The interface declares a single method, `Filter(IQueryable<T> query)`, which accepts an `IQueryable<T>` to which the filtering criteria are to be applied. How that is done is completely up to the implementor, but typically it will be something along the lines of:

```csharp
public class PersonByNameFilterCriteria : IFilterCriteria<Person>
{
    private readonly string _familyName;
    private readonly string? _givenName;
    
    public PersonByNameFilterCriteria(string familyName, string? givenName = default)
    {
        _familyName = familyName;
        _givenName = givenName;
    }
    
    public IQueryable<User> Filter(IQueryable<User> query) 
    {
        query = query.Where(p => p.FamilyName.Equals(_familyName));
            
        if (!string.IsNullOrEmpty(_givenName))
        {
            query = query.Where(p => p.GivenName.Equals(_givenName));
        }
        
        return query;
    }
}
```

The `ListAsync()` method has numerous overloads and extensions that accept different parameters in order to customize the returned result. Among these overloads are those that accept an [`IPaginator<T>`](Developist.Core.Persistence/Pagination/IPaginator`1.cs) to partition large result sets, and those that accept an `IFilterCriteria<T>` to filter the data based on specific criteria.
For impromptu queries, the package also offers extension methods that wrap a predicate expression, `Expression<Func<T, bool>>`, within an [`PredicateFilterCriteria<T>`](Developist.Core.Persistence/Filtering/PredicateFilterCriteria`1.cs) instance.

#### Persisting changes

Changes made to any entities that were retrieved through a repository that was obtained from a unit of work, will be committed back to the database upon calling that unit of work's `CompleteAsync()` method.

New entities can be added using the repository's `Add()` method and existing entities can be removed using its `Remove()` method. Again, these changes will only be persisted after calling the unit of work's `CompleteAsync()` method.
