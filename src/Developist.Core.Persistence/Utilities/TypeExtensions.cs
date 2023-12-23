namespace Developist.Core.Persistence.Utilities;

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

    /// <summary>
    /// Checks if the type is concrete (i.e., not an interface or abstract class).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the type is concrete; otherwise, <see langword="false"/>.</returns>
    public static bool IsConcrete(this Type type)
    {
        return !type.IsInterface && !type.IsAbstract;
    }

    /// <summary>
    /// Checks if the type implements a generic interface.
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="genericTypeDefinition">The generic type definition to look for.</param>
    /// <returns><see langword="true"/> if the type implements the generic interface; otherwise, <see langword="false"/>.</returns>
    public static bool ImplementsGenericInterface(this Type type, Type genericTypeDefinition)
    {
        return type.GetInterfaces().Any(
            iface => iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(genericTypeDefinition));
    }

    /// <summary>
    /// Checks if the type has a generic type argument of a specific type.
    /// </summary>
    /// <typeparam name="TArg">The generic type argument to check for.</typeparam>
    /// <param name="type">The type to check.</param>
    /// <returns><see langword="true"/> if the type has the specified generic type argument; otherwise, <see langword="false"/>.</returns>
    public static bool HasGenericTypeArgument<TArg>(this Type type)
    {
        return type.GetGenericArguments().Any(arg => arg.Equals(typeof(TArg)));
    }
}
