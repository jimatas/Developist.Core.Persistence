using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a unit of work interface specific to Entity Framework Core.
/// </summary>
public interface IUnitOfWork : IUnitOfWorkBase
{
    /// <summary>
    /// Gets the underlying database context associated with the unit of work.
    /// </summary>
    DbContext DbContext { get; }
}
