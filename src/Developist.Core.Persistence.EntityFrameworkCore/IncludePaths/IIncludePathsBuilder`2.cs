namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a builder for nested include paths of entities of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <typeparam name="TProperty">The type of the property that is being included.</typeparam>
public interface IIncludePathsBuilder<T, out TProperty> : IIncludePathsBuilder<T>
{
    /// <summary>
    /// Includes a path for related entities that are nested under the previously included related entities in the query result.
    /// </summary>
    /// <param name="pathSegment">The path segment to include.</param>
    void ThenInclude(string pathSegment);
}
