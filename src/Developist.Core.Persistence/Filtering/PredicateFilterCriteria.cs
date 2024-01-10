using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence;

/// <summary>
/// An implementation of the <see cref="IFilterCriteria{T}"/> interface that filters data using a predicate.
/// </summary>
/// <typeparam name="T">The type of data to filter.</typeparam>
public class PredicateFilterCriteria<T> : IFilterCriteria<T>
{
    private readonly Expression<Func<T, bool>> _predicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="PredicateFilterCriteria{T}"/> class with the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to use for filtering data.</param>
    public PredicateFilterCriteria(Expression<Func<T, bool>> predicate)
    {
        _predicate = Ensure.Argument.NotNull(predicate);
    }

    /// <inheritdoc/>
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        return query.Where(_predicate);
    }
}
