# Developist.Core.Persistence

This library offers persistence base classes and interfaces, including well-defined implementations of the Repository and Unit of Work patterns, as well as an Entity Framework Core implementation.

## Examples

The following code sample showcases standard usage patterns for [`IUnitOfWorkBase`](./src/Developist.Core.Persistence/IUnitOfWorkBase.cs) and [`IRepository<T>`](./src/Developist.Core.Persistence/IRepository.cs).

```csharp
// Unit of Work obtained through dependency injection.
IUnitOfWorkBase _unitOfWork = ...;

// Repository creation for a specific entity.
IRepository<Customer> customerRepository = _unitOfWork.Repository<Customer>();

// Asynchronous data retrieval with predicate criteria.
IPaginatedList<Customer> customers = await customerRepository.ListAsync(
    cust => cust.PaymentMethod == PaymentMethod.Ideal,
    p => p.StartAtPage(1).UsePageSize(10).SortBy(c => c.CustomerNumber));

foreach (var customer in customers)
{
    ChangePaymentMethod(customer, PaymentMethod.DirectDebit);
}

// Saving modified entities back to the database.
await _unitOfWork.CompleteAsync();
```

The sample below illustrates how to configure and register an [`IUnitOfWork<TContext>`](./src/Developist.Core.Persistence.EntityFrameworkCore/IUnitOfWork`1.cs) for Entity Framework Core with the dependency injection container.

```csharp
// Register a typed DbContext with DI if additional configuration is needed.
// Otherwise, the AddUnitOfWork call alone suffices.
services.AddDbContext<CustomContext>(options => options.UseSqlServer("...."));

// Register the Unit of Work for the CustomContext with DI.
services.AddUnitOfWork<CustomContext>();
```

The following code sample demonstrates how to efficiently retrieve related entities using one of the [extensibility methods](./src/Developist.Core.Persistence.EntityFrameworkCore/QueryExtenders/RepositoryExtensions.WithIncludes.cs) provided by [`Repository<T>`](./src/Developist.Core.Persistence.EntityFrameworkCore/Repository.cs).

```csharp
// Create a repository instance with included related entities.
var repositoryWithIncludes = _unitOfWork.Repository<Customer>()
  .WithIncludes(props => props.Include(cust => cust.PaymentInformation).ThenInclude(pay => pay.AccountDetails));

// Retrieve a customer entity by its unique identifier.
var customer = repositoryWithIncludes.SingleOrDefaultAsync(cust => cust.Id == id);

// Access information from the related entities.
var bankAccountNumber = customer.PaymentInformation.AccountDetails.BankAccountNumber;
```
