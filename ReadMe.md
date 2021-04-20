# Developist.Core.Persistence
Lightweight persistence library providing Entity Framework Core and in-memory (through HashSet) implementations of the Repository and Unit of Work patterns.

## Quick start
1. Define your entities by implementing either the `IEntity` or `IEntity<TIdentifier>` interface, or alternatively, by inheriting from the `EntityBase<TIdentifier>` base class.

```csharp
public class Person : EntityBase<int>
{
    public string GivenName { get; set; } 
    public string FamilyName { get; set; } 
    public DateTime BirthDate { get; set; } 
    // Additional state and behavior...
}
```

2. Define your `DbContext` subclass, including any custom configuration that is needed to map your entities to the database schema, and register it with the dependency injection system using the `AddDbContext` method.

```csharp
public class MyDbContext : DbContext
{
   public DbSet<Person> People { get; set; }
   // Other entity sets...
}

// In the ConfigureServices method of your Startup.cs:
services.AddDbContext<MyDbContext>(options => options.UseSqlServer("MyDbConnectionString"));
```

3. Register the `IUnitOfWork` and `IRepositoryFactory` dependencies with the built-in dependency injection container. You can do this by using the `AddPersistence<TDbContext>` extension method, which is provided for convenience, or by manually adding the dependencies through the `IServiceCollection`'s `Add` methods.

```csharp
// To use the Entity Framework Core version:
services.AddPersistence<MyDbContext>();

// Or to use the in-memory version:
services.AddPersistence();
```

You can optionally specify a custom `IRepositoryFactory` type, which will be used instead of the default factory to create the repositories that are returned by the `IUnitOfWork`'s `Repository<TEntity>()` method. The other optional parameter specifies the lifetime to register the `IUnitOfWork` dependency with. The default lifetime is `ServiceLifetime.Scoped`.

## Usage
A typical usage scenario involves injecting the `IUnitOfWork` dependency through the consumer's constructor. You can subsequently query for entities and persist them using the `IRepository<TEntity>` instances that are obtained through the `IUnitOfWork`'s `Repository<TEntity>()` method as in the following example.

```csharp
public ConsumingService(IUnitOfWork uow) 
{
    this.uow = uow;
}

public void EnumeratePeopleWithName(string familyName) 
{
    IQueryableFilter<Person> byFamilyName = new PeopleByName { FamilyName = familyName };
    
    IEnumerable<Person> people = uow.Repository<Person>().Find(byFamilyName);
}
```

The `IQueryableFilter<T>` interface is essentially an implementation of the Specification pattern. The interface exposes a single method, `Filter(IQueryable<T> sequence)`, which accepts an `IQueryable<T>` to which the filtering criteria are to be applied. How that is done is completely up to the implementor, but typically it will be something along the lines of.

```csharp
public class PeopleByName : IQueryableFilter<T>
{
    // The filtering criteria are simply properties.
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    
    public IQueryable<T> Filter(IQueryable<T> sequence)
    {
        if (GivenName is not null)
        {
            sequence = sequence.Where(p => p.GivenName.Equals(GivenName));
        }
        
        if (FamilyName is not null)
        {
            sequence = sequence.Where(p => p.FamilyName.Equals(FamilyName));
        }

        return sequence;
    }
}
```
The `Find` method and its async counterpart, `FindAsync`, have numerous overloads that accept different parameters in order to customize the returned result. Among these overloads are those that accept an `IQueryablePaginator<T>`, to partition large result sets, and those that accept an `IEntityIncludePaths<TEntity>`, through which any related data to eager load can be specified.

For example, suppose the following simplified model.

```csharp
public class Book : EntityBase<Guid>
{
    public string Title { get; set; }
    public Author Author { get; set; }
}

public class Author : EntityBase<Guid>
{
    public string Name { get; set; }
    public IEnumerable<Book> Books { get; set; }
}
```

The following query will then retrieve a `Book` entity by its title using a predicate expression. The book that is returned will have its `Author` navigation property loaded, as well as all the items in the `Books` navigation property of that `Author`.

```csharp
var includePaths = EntityIncludePaths.ForEntity<Book>()
    .Include(b => b.Author)
        .ThenInclude(a => a.Books);

var book = repository.Find(b => b.Title.Equals("The Old Man and the Sea"), includePaths).Single();
var allBooksByHemingway = book.Author.Books;
```

For impromptu queries, such as the one above, there's some extension methods provided that wrap a predicate expression in an `IQueryableFilter<T>` instance. To further assist with this, there's also a couple of extension methods provided for logically combining predicates using AND (`AndAlso`) and OR (`OrElse`).

### Persisting changes
Any changes made to the entities that have been retrieved through the `IRepository<TEntity>` instance will be committed back to the database upon calling the `IUnitOfWork`'s `Complete` method, or its async counterpart, `CompleteAsync`.

New entities can be added using the `IRepository<TEntity>`'s `Add` method and existing entities can be removed using its `Remove` method. Again, these changes will only be persisted after calling the `IUnitOfWork`'s `Complete` or `CompleteAsync` method.

## Further examples
For more examples, please see the unit tests and integration tests as well as the samples project inside the solution.

## `// TODO:`
Add support for transactions on the underlying `DbContext` for `IUnitOfWork<TDbContext>`. Either explicitly (muddying up the interface) or implicitly, by hooking into the `Completed` event and starting up a new one after each call to `Complete`.
