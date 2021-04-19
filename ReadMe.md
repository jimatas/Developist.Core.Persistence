# Developist.Core.Persistence
Lightweight persistence library providing Entity Framework Core and in-memory (through HashSet) implementations of the Repository and Unit of Work patterns.  
Targets .NET 5.0

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
   // other entity sets...
}

// In ConfigureServices:
services.AddDbContext<MyDbContext>(options => options.UseSqlServer("MyDbConnectionString"));
```

3. Register the `IUnitOfWork` and `IRepositoryFactory` dependencies with the built-in dependency injection container. You can do this by using the `AddPersistence<TDbContext>` extension method, which is provided for convenience, or by manually adding the dependencies through the `IServiceCollection`'s Add methods.

```csharp
// To use the Entity Framework Core version:
services.AddPersistence<MyDbContext>();

// Or to use the in-memory version:
services.AddPersistence();
```

You can optionally specify a custom `IRepositoryFactory` type, which will be used instead of the default factory to create the repositories that are returned by the `IUnitOfWork`'s `Repository<TEntity>()` method. The other optional parameter specifies the lifetime to register the `IUnitOfWork` dependency with. The default lifetime is `ServiceLifetime.Scoped`.

## Usage
A typical usage scenario involves injecting the `IUnitOfWork` dependency through the consumer's constructor and subsequently querying for entities, and persisting them, using the `IRepository<TEntity>` instances that are obtained through the `IUnitOfWork`'s `Repository<TEntity>()` method as in the following example.

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

