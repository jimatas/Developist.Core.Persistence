using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.Utilities;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a repository for managing entities of type <typeparamref name="T"/> in an Entity Framework Core data store.
/// </summary>
/// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
public class Repository<T> : IRepository<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T}"/> class with the specified unit of work.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to use for managing changes to the data store.</param>
    public Repository(IUnitOfWork unitOfWork)
    {
        UnitOfWork = Ensure.NotNull(unitOfWork);
    }

    /// <summary>
    /// Gets the Entity Framework Core specific unit of work associated with this repository.
    /// </summary>
    public IUnitOfWork UnitOfWork { get; }

    /// <inheritdoc/>
    IUnitOfWorkBase IRepository<T>.UnitOfWork => UnitOfWork;

    /// <inheritdoc/>
    public void Add(T entity)
    {
        Ensure.NotNull(entity);
        UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
    }

    /// <inheritdoc/>
    public void Remove(T entity)
    {
        Ensure.NotNull(entity);
        UnitOfWork.DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(CancellationToken cancellationToken = default)
    {
        return CreateQuery().AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<bool> AnyAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().Filter(filterCriteria).AnyAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return CreateQuery().CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().Filter(filterCriteria).CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<T?> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return CreateQuery().FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<T?> FirstOrDefaultAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().Filter(filterCriteria).FirstOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<T?> SingleOrDefaultAsync(IFilterCriteria<T> filterCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().Filter(filterCriteria).SingleOrDefaultAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> ListAsync(IPaginationCriteria<T> paginationCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().ToPaginatedListAsync(paginationCriteria, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> ListAsync(IFilterCriteria<T> filterCriteria, IPaginationCriteria<T> paginationCriteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().Filter(filterCriteria).ToPaginatedListAsync(paginationCriteria, cancellationToken);
    }

    /// <summary>
    /// Creates a queryable object for querying entities of type <typeparamref name="T"/> in the associated database context.
    /// </summary>
    /// <returns>The queryable object for querying entities.</returns>
    protected virtual IQueryable<T> CreateQuery()
    {
        return UnitOfWork.DbContext.Set<T>().AsQueryable();
    }
}
