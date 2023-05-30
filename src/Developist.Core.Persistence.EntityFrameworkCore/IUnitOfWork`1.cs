using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Defines a unit of work that provides access to a database context of type <typeparamref name="TContext"/>.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public interface IUnitOfWork<TContext> : IUnitOfWork where TContext : DbContext
{
    /// <summary>
    /// Gets the database context of type <typeparamref name="TContext"/> associated with the unit of work.
    /// </summary>
    TContext DbContext { get; }
}
