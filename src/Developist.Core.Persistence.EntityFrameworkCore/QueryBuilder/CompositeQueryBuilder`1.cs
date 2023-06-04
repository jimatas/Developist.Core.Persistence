namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a composite query builder implementation that combines multiple query builders.
/// </summary>
/// <typeparam name="T">The entity type being queried.</typeparam>
internal class CompositeQueryBuilder<T> : IQueryBuilder<T> where T : class
{
    private readonly IEnumerable<IQueryBuilder<T>> _queryBuilders;

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositeQueryBuilder{T}"/> class with the specified query builders.
    /// </summary>
    /// <param name="queryBuilders">The query builders to compose.</param>
    public CompositeQueryBuilder(params IQueryBuilder<T>[] queryBuilders)
    {
        _queryBuilders = queryBuilders ?? throw new ArgumentNullException(nameof(queryBuilders));
    }

    /// <inheritdoc/>
    public IQueryable<T> BuildQuery(IQueryable<T> query)
    {
        foreach (var builder in _queryBuilders)
        {
            query = builder.BuildQuery(query);
        }

        return query;
    }
}
