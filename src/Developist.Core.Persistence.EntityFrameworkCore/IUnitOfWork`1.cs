using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a unit of work interface specific to Entity Framework Core with a specific database context type.
/// </summary>
/// <typeparam name="TContext">The type of the database context associated with the unit of work.</typeparam>
public interface IUnitOfWork<TContext> : IUnitOfWork
    where TContext : DbContext
{
    /// <summary>
    /// Gets the underlying database context of type <typeparamref name="TContext"/> associated with the unit of work.
    /// </summary>
    new TContext DbContext { get; }
}
