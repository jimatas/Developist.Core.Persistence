# Developist.Core.Persistence

### Entity Framework Core and in-memory Unit of Work and Repository implementations

Start by defining your entities by deriving from the [`IEntity`](Developist.Core.Persistence/Entities/IEntity.cs) or [`IEntity<TIdentifier>`](Developist.Core.Persistence/Entities/IEntity`1.cs) interface, or alternatively from the [`EntityBase<TIdentifier>`](Developist.Core.Persistence/Entities/EntityBase`1.cs) abstract class.

If you are using the Entity Framework Core version of the package, you will also have to define your `DbContext` subclass and any entity type configurations that are needed to map your entities to the database schema.

Register the [`IUnitOfWork`](Developist.Core.Persistence/IUnitOfWork.cs) depdendency with the built-in dependency injection framework by using the appropriate DI-registration extension method: either `AddUnitOfWork<TDbContext>()` for the Entity Framework Core version, or `AddUnitOfWork()` for the in-memory version of the package.

There are some overloads of this extension method provided that accept an [`IRepositoryFactory`](Developist.Core.Persistence/IRepositoryFactory.cs) type or instance, which will be used instead of the default factory to create the [`IRepository<TEntity>`](Developist.Core.Persistence/IRepository`1.cs) instances returned by the unit of work's `Repository<TEntity>()` method.

#### Usage
A typical usage scenario involves injecting the `IUnitOfWork` interface through the constructor of a consumer. You can subsequently query entities and persist them using the repositories obtained through the unit of work's `Repository<TEntity>` method, such as in the following example.

```csharp
public UserFinderService(IUnitOfWork uow) => this.uow = uow;

public Task<User?> GetUserByUsername(string username)
{
    IQueryableFilter<User> filterByUsername = new UsernameQueryableFilter(username);
    var matchingUsers = await uow.Repository<User>().FindAsync(filterByUsername);
    return matchingUsers.SingleOrDefault();
}
```

The [`IQueryableFilter<TEntity>`](Developist.Core.Persistence/IQueryableFilter`1.cs) interface in the previous example is an implementation of the Specification pattern. The interface declares a single method, `Filter(IQueryable<TEntity> sequence)`, which accepts an `IQueryable<T>` to which the filtering criteria are to be applied. How that is done is completely up to the implementor, but typically it will be something along the lines of:

```csharp
public class UsernameQueryableFilter : IQueryableFilter<User>
{
    private readonly string? username;
    public UsernameQueryableFilter(string username) => this.username = username;
    public IQueryable<User> Filter(IQueryable<User> sequence) 
    {
        if (!string.IsNullOrEmpty(this.username))
        {
            sequence = sequence.Where(u => u.Username.Equals(username));
        }
        return sequence;
    }
}
```

The `FindAsync` method has numerous overloads and extensions that accept different parameters in order to customize the returned result. Among these overloads are those that accept an [`IQueryablePaginator<TEntity>`](Developist.Core.Persistence/Pagination/IQueryablePaginator`1.cs) to partition large result sets, and those that accept an [`IIncludePathsBuilder<TEntity>`](Developist.Core.Persistence/Entities/IncludePaths/IIncludePathsBuilder`1.cs) through which the include paths of related entities to eager load can be specified, as for instance in the following example.
```csharp
var userWithLoginHistory = (await repository.FindAsync(filterByUsername, 
    related => related.Include(u => u.AccountInfo).ThenInclude(a => a.LoginHistory))).SingleOrDefault();
```

For impromptu queries there's some extension methods provided that wrap a predicate expression in an `IQueryableFilter<TEntity>` instance. To further assist with this, there's also a couple of extension methods provided for logically combining predicates using AND (`AndAlso`) and OR (`OrElse`).

#### Persisting changes

Changes made to any entities that were retrieved through a repository that was obtained from a unit of work, will be committed back to the database upon calling that unit of work's `CompleteAsync` method.

New entities can be added using the repository's `Add` method and existing entities can be removed using its `Remove` method. Again, these changes will only be persisted after calling the unit of work's `CompleteAsync` method.
