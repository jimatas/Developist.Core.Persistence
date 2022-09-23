using System;
using System.Linq;

namespace Developist.Core.Persistence.Utilities
{
    internal static class TypeExtensions
    {
        public static bool IsConcrete(this Type type) => !(type.IsInterface || type.IsAbstract);

        public static bool ImplementsGenericInterface(this Type type, Type genericTypeDefinition)
        {
            return type.FindInterfaces((candidate, criteria) => candidate.IsGenericType && candidate.GetGenericTypeDefinition().Equals(criteria), genericTypeDefinition).Any();
        }
    }
}
