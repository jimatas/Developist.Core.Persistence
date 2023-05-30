using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.IncludePaths;
using Developist.Core.Persistence.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a repository for managing entities of type <typeparamref name="T"/> in an Entity Framework Core data store.
/// </summary>
/// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
/// <typeparam name="TContext">The type of the database context.</typeparam>
public class Repository<T, TContext> : IRepository<T>
    where T : class
    where TContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{T, TContext}"/> class with the specified unit of work.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to use for managing changes to the data store.</param>
    public Repository(IUnitOfWork<TContext> unitOfWork)
    {
        UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    /// <inheritdoc/>
    public IUnitOfWork UnitOfWork { get; }

    /// <summary>
    /// Gets the database context associated with the current unit of work.
    /// </summary>
    protected DbContext DbContext => ((IUnitOfWork<TContext>)UnitOfWork).DbContext;

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
    public Task<IPaginatedList<T>> ListAsync(
        IPaginator<T> paginator,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>().ToPaginatedListAsync(paginator, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> ListAsync(
        IPaginator<T> paginator,
        IIncludePathsBuilder<T> includePaths,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>().WithIncludes(includePaths).ToPaginatedListAsync(paginator, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> FindAsync(
        IFilterCriteria<T> criteria,
        IPaginator<T> paginator,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>().FilterBy(criteria).ToPaginatedListAsync(paginator, cancellationToken);
    }

    /// <inheritdoc/>
    public Task<IPaginatedList<T>> FindAsync(
        IFilterCriteria<T> criteria,
        IPaginator<T> paginator,
        IIncludePathsBuilder<T> includePaths,
        CancellationToken cancellationToken = default)
    {
        return DbContext.Set<T>()
            .WithIncludes(includePaths)
            .FilterBy(criteria)
            .ToPaginatedListAsync(paginator, cancellationToken);
    }
}
