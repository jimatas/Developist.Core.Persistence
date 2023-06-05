using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

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
    public Repository(IUnitOfWorkBase unitOfWork)
    {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <summary>
    /// Gets the Entity Framework Core specific unit of work associated with this repository.
    /// </summary>
    public IUnitOfWorkBase UnitOfWork { get; }

    /// <inheritdoc/>
    IUnitOfWork IRepository<T>.UnitOfWork => UnitOfWork;

    /// <summary>
    /// Gets the database context associated with the current unit of work.
    /// </summary>
    protected DbContext DbContext => UnitOfWork.DbContext;

    /// <inheritdoc/>
    public void Add(T entity)
    {
        DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Added;
    }

    /// <inheritdoc/>
    public void Remove(T entity)
    {
        DbContext.Entry(entity, attachIfDetached: true).State = EntityState.Deleted;
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>().CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    public Task<int> CountAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>().FilterBy(criteria).CountAsync(cancellationToken);
    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public Task<T> SingleOrDefaultAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().FilterBy(criteria).SingleOrDefaultAsync(cancellationToken)!;
    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public Task<T> FirstOrDefaultAsync(CancellationToken cancellationToken = default)
    {
        return CreateQuery().FirstOrDefaultAsync(cancellationToken)!;
    }

    /// <inheritdoc/>
    [return: MaybeNull]
    public Task<T> FirstOrDefaultAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default)
    {
        return CreateQuery().FilterBy(criteria).FirstOrDefaultAsync(cancellationToken)!;
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> ListAsync(
        IPaginator<T> paginator,
        CancellationToken cancellationToken = default)
    {
        return CreateQuery().ToPaginatedListAsync(paginator, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> ListAsync(
        IFilterCriteria<T> criteria,
        IPaginator<T> paginator,
        CancellationToken cancellationToken = default)
    {
        return CreateQuery().FilterBy(criteria).ToPaginatedListAsync(paginator, cancellationToken);
    }

    /// <summary>
    /// Creates a queryable object for querying entities of type <typeparamref name="T"/> in the associated database context.
    /// </summary>
    /// <returns>The queryable object for querying entities.</returns>
    protected virtual IQueryable<T> CreateQuery()
    {
        return DbContext.Set<T>();
    }
}
