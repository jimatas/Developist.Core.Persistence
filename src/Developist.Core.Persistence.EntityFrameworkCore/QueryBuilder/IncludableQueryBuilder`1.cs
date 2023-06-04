namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a query builder implementation that adds include paths to a query.
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
internal class IncludableQueryBuilder<T> : IQueryBuilder<T> where T : class
{
    private readonly IIncludePathsBuilder<T> _includePaths;

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludableQueryBuilder{T}"/> class with the specified include paths builder.
    /// </summary>
    /// <param name="includePaths">The include paths builder.</param>
    public IncludableQueryBuilder(IIncludePathsBuilder<T> includePaths)
    {
        _includePaths = includePaths ?? throw new ArgumentNullException(nameof(includePaths));
    }

    /// <inheritdoc/>
    public IQueryable<T> BuildQuery(IQueryable<T> query)
    {
        return query.WithIncludes(_includePaths);
    }
}
