namespace Developist.Core.Persistence;

/// <summary>
/// Defines the criteria for filtering data of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of data the filter criteria applies to.</typeparam>
public interface IFilterCriteria<T> : ICriteria<T>
{
}
