namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a builder for nested include paths of entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <typeparam name="TProperty">The type of the property that is being included.</typeparam>
public interface IIncludesBuilder<T, out TProperty> : IIncludesBuilder<T>
{
    /// <summary>
    /// Specifies the path segment of a related entity to include in the query result, which is nested under a previously included related entity.
    /// </summary>
    /// <param name="pathSegment">The path segment of a nested related entity to include.</param>
    void ThenInclude(string pathSegment);
}
