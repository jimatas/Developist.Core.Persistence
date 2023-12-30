namespace Developist.Core.Persistence;

/// <summary>
/// Defines the criteria for filtering data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data the filter criteria applies to.</typeparam>
public interface IFilterCriteria<T> : ICriteria<T>
{
    /// <summary>
    /// Applies the filter criteria to the provided queryable data source.
    /// </summary>
    /// <param name="query">An <see cref="IQueryable{T}"/> representing the data source to
    /// which the filter criteria will be applied.</param>
    /// <returns>An <see cref="IQueryable{T}"/> that contains elements from the input query
    /// that meet the conditions specified by the filter criteria.</returns>
    new IQueryable<T> Apply(IQueryable<T> query);
}
