using Developist.Core.ArgumentValidation;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Represents a default implementation of <see cref="IIncludesBuilder{T, TProperty}"/>.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
/// <typeparam name="TProperty">The type of the property that is being included.</typeparam>
internal class IncludesBuilder<T, TProperty> : IncludesBuilder<T>, IIncludesBuilder<T, TProperty>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IncludesBuilder{T, TProperty}"/> class.
    /// </summary>
    /// <param name="paths">The list of included paths.</param>
    public IncludesBuilder(IList<string> paths)
        : base(paths) { }

    /// <inheritdoc/>
    public void ThenInclude(string pathSegment)
    {
        if (!Paths.Any())
        {
            throw new InvalidOperationException($"The '{nameof(ThenInclude)}' method requires an initial path to be set before it can be used.");
        }

        Ensure.Argument.NotNullOrWhiteSpace(pathSegment);
        Paths[^1] += $".{pathSegment}";
    }
}
