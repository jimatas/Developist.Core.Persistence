namespace Developist.Customers.Api.Serialization;

internal static class TypeExtensions
{
    public static bool ImplementsGenericInterface(this Type type, Type genericTypeDefinition)
    {
        return type.GetInterfaces().Any(
            iface => iface.IsGenericType && iface.GetGenericTypeDefinition().Equals(genericTypeDefinition));
    }
}
