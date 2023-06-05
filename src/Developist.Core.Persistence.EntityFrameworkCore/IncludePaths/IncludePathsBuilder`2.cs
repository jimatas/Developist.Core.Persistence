namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides a builder for constructing include paths for a given entity type and property.
/// </summary>
/// <typeparam name="T">The type of entity for which to include paths.</typeparam>
/// <typeparam name="TProperty">The type of property to include.</typeparam>
public class IncludePathsBuilder<T, TProperty> : IncludePathsBuilder<T>, IIncludePathsBuilder<T, TProperty>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludePathsBuilder{T, TProperty}"/> class with the specified list of paths.
    /// </summary>
    /// <param name="paths">The list of paths to include.</param>
    internal IncludePathsBuilder(IList<string> paths) : base(paths) { }

    /// <inheritdoc/>
    public void ThenInclude(string pathSegment)
    {
        if (!Paths.Any())
        {
            throw new InvalidOperationException($"The '{nameof(ThenInclude)}' method cannot be called before at least one path has been included.");
        }

        if (string.IsNullOrWhiteSpace(pathSegment))
        {
            throw new ArgumentException(
                message: "Value cannot be null, empty, or contain only whitespace characters.",
                paramName: nameof(pathSegment));
        }

        Paths[^1] += $".{pathSegment}";
    }
}
