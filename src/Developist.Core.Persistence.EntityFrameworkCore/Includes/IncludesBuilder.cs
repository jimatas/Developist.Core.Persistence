using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a default implementation of <see cref="IIncludesBuilder{T}"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
internal class IncludesBuilder<T> : IIncludesBuilder<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludesBuilder{T}"/> class.
    /// </summary>
    public IncludesBuilder()
        : this(new List<string>()) { }

    /// <summary>
    /// Initializes a new instance of the <see cref="IncludesBuilder{T}"/> class with a list of included paths.
    /// </summary>
    /// <param name="paths">The list of included paths.</param>
    protected IncludesBuilder(IList<string> paths) => Paths = paths;

    /// <summary>
    /// Gets the list of included paths.
    /// </summary>
    protected IList<string> Paths { get; }

    /// <inheritdoc/>
    public void Include(string path)
    {
        Ensure.Argument.NotNullOrWhiteSpace(path);
        Paths.Add(path);
    }

    /// <inheritdoc/>
    public IList<string> AsList() => Paths;
}
