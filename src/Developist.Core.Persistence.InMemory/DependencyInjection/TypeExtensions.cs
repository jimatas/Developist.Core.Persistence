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
    }
}
