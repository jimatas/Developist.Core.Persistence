namespace Developist.Core.Persistence;

/// <summary>
/// Provides extension methods for working with types.
/// </summary>
internal static class TypeExtensions
{
    /// <summary>
    /// Gets a property by name, ignoring case.
    /// </summary>
    /// <param name="type">The type to search for the property in.</param>
    /// <param name="propertyName">The name of the property to retrieve.</param>
    /// <returns>The <see cref="PropertyInfo"/> object representing the property, or <see langword="null"/> if not found.</returns>
    public static PropertyInfo? GetPropertyIgnoreCase(this Type type, string propertyName)
    {
        return type.GetProperty(propertyName,
            BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.IgnoreCase);
    }
}
