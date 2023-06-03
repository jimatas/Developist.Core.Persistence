namespace System
{
    /// <summary>
    /// A static class containing extension methods for the <see cref="Type"/> class.
    /// </summary>
    internal static class TypeExtensions
    {
        /// <summary>
        /// Determines whether the specified <see cref="Type"/> object represents a concrete (i.e. non-abstract and non-interface) type.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object to check.</param>
        /// <returns><c>true</c> if the specified type is concrete; otherwise, <c>false</c>.</returns>
        public static bool IsConcrete(this Type type) => !(type.IsInterface || type.IsAbstract);

        /// <summary>
        /// Gets an array of <see cref="Type"/> objects representing the implemented generic interfaces of the specified type that match the specified generic type definition.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> object to check for implemented generic interfaces.</param>
        /// <param name="genericTypeDefinition">The <see cref="Type"/> object representing the generic type definition to match.</param>
        /// <returns>An array of <see cref="Type"/> objects representing the implemented generic interfaces of the specified type that match the specified generic type definition.</returns>
        public static Type[] GetImplementedGenericInterfaces(this Type type, Type genericTypeDefinition)
        {
            return type.FindInterfaces(
                filter: (candidate, criteria) => candidate.IsGenericType && candidate.GetGenericTypeDefinition().Equals(criteria),
                filterCriteria: genericTypeDefinition);
        }
    }
}
