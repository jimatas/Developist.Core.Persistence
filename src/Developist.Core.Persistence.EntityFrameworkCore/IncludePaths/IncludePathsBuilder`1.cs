namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides a builder for constructing include paths for a given entity type.
/// </summary>
/// <typeparam name="T">The type of entity for which to include paths.</typeparam>
public class IncludePathsBuilder<T> : IIncludePathsBuilder<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludePathsBuilder{T}"/> class with an empty list of paths.
    /// </summary>
    public IncludePathsBuilder() : this(new List<string>()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludePathsBuilder{T}"/> class with the specified list of paths.
    /// </summary>
    /// <param name="paths">The list of paths to include.</param>
    protected IncludePathsBuilder(IList<string> paths) => Paths = paths;

    /// <summary>
    /// Gets the list of paths that have been included.
    /// </summary>
    protected IList<string> Paths { get; }

    /// <inheritdoc/>
    public void Include(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            throw new ArgumentException(
                message: "Value cannot be null, empty, or contain only whitespace characters.",
                paramName: nameof(path));
        }

        Paths.Add(path);
    }

    /// <inheritdoc/>
    public IList<string> AsList() => Paths;
}
