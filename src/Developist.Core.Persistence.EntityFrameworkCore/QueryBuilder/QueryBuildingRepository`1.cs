namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a repository with dynamically extensible query building capabilities.
/// </summary>
/// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
internal class QueryBuildingRepository<T> : Repository<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QueryBuildingRepository{T}"/> class with the specified unit of work and query builder.
    /// </summary>
    /// <param name="unitOfWork">The unit of work to use for managing changes to the data store.</param>
    /// <param name="queryBuilder">The query builder to use for extending and modifying queries.</param>
    public QueryBuildingRepository(IUnitOfWorkBase unitOfWork, IQueryBuilder<T> queryBuilder)
        : base(unitOfWork)
    {
        QueryBuilder = queryBuilder ?? throw new ArgumentNullException(nameof(queryBuilder));
    }

    /// <summary>
    /// Gets the query builder used by the repository for extending and modifying queries.
    /// </summary>
    public IQueryBuilder<T> QueryBuilder { get; }

    /// <inheritdoc/>
    protected override IQueryable<T> CreateQuery()
    {
        return QueryBuilder.BuildQuery(base.CreateQuery());
    }
}
