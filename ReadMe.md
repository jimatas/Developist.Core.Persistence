# Developist.Core.Persistence
Persistence library providing Entity Framework Core and in-memory (through HashSet) implementations of the Repository and Unit of Work patterns.  
Targets .NET 5.0

## Quick start
1. Define your entities by implementing either the `IEntity` or `IEntity<TIdentifier>` interface, or alternatively, by inheriting from the `EntityBase<TIdentifier>` base class.

```csharp
public class Person : EntityBase<int>
{
    public string GivenName { get; set; } 
    public string FamilyName { get; set; } 
    public DateTime BirthDate { get; set; } 
}
```

2. Define your `DbContext` subclass and any custom configuration that is needed to map your entities to the database schema.

```csharp
public class MyDbContext : DbContext
{
   public DbSet<Person> People { get; set; }
   // other entity sets...
}
```

3. Register the `IUnitOfWork` and `IRepositoryFactory` dependencies with the built-in dependency injection container. You can do this by using the `AddPersistence<TDbContext>` extension method, which is provided for convenience, or by manually adding the dependencies through the `IServiceCollection`'s Add methods.

```csharp
// To use the Entity Framework version:
services.AddPersistence<MyDbContext>();

// Or to use the in-memory version:
services.AddPersistence(ServiceLifetime.Transient);
```

You can optionally specify a custom `IRepositoryFactory` type, which will be used instead of the default factory to create the repositories that are returned by the `IUnitOfWork`'s `Repository<TEntity>()` method. The other optional parameter specifies the lifetime to register the `IUnitOfWork` dependency with. The default lifetime is `ServiceLifetime.Scoped`.
