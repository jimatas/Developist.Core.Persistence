using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Defines a repository factory that creates repositories for a specific database context type.
/// </summary>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public interface IRepositoryFactory<TContext> where TContext : DbContext
{
    /// <summary>
    /// Creates a repository for the specified entity type and unit of work.
    /// </summary>
    /// <typeparam name="T">The type of entity for which to create a repository.</typeparam>
    /// <param name="unitOfWork">The unit of work associated with the repository.</param>
    /// <returns>A repository for the specified entity type and unit of work.</returns>
    IRepository<T> Create<T>(IUnitOfWork<TContext> unitOfWork) where T : class;
}
