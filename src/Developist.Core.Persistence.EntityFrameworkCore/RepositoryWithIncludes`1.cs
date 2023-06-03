namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a repository implementation with support for including related entities, derived from the base <see cref="Repository{T}"/> class.
/// </summary>
/// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
internal class RepositoryWithIncludes<T> : Repository<T> where T : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryWithIncludes{T}"/> class with the specified unit of work and include paths builder.
    /// </summary>
    /// <param name="unitOfWork">The unit of work associated with the repository.</param>
    /// <param name="includePathsBuilder">The include paths builder for eager loading related entities.</param>
    public RepositoryWithIncludes(IUnitOfWorkBase unitOfWork, IIncludePathsBuilder<T> includePathsBuilder)
        : base(unitOfWork)
    {
        IncludePathsBuilder = includePathsBuilder ?? throw new ArgumentNullException(nameof(includePathsBuilder));
    }

    /// <summary>
    /// Gets the include paths builder for eager loading related entities.
    /// </summary>
    public IIncludePathsBuilder<T> IncludePathsBuilder { get; }

    /// <inheritdoc/>
    protected override IQueryable<T> CreateQuery()
    {
        return base.CreateQuery().WithIncludes(IncludePathsBuilder);
    }
}
