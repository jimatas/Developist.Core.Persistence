// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;

namespace Developist.Core.Persistence
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Returns a boolean value indicating whether the current type represents a concrete (instantiable) class.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><see langword="true"/> or <see langword="false"/>, as the type either represents a concrete class or not.</returns>
        public static bool IsConcrete(this Type type) => type.IsClass && !type.IsAbstract;

        /// <summary>
        /// Returns a boolean value that indicates whether the current type derives from a generic parent with the specified generic type definition.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <param name="genericTypeDefinition">The generic type definition of the generic parent to check for derivation from.</param>
        /// <returns><see langword="true"/> or <see langword="false"/>, as the type either derives from the specified generic parent or not.</returns>
        public static bool DerivesFromGenericParent(this Type type, Type genericTypeDefinition)
        {
            if (type == typeof(object))
            {
                return false;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return true;
            }

            if (type.IsInterface)
            {
                return false;
            }

            return type.BaseType.DerivesFromGenericParent(genericTypeDefinition)
                || type.GetInterfaces().Any(iface => iface.DerivesFromGenericParent(genericTypeDefinition));
        }
    }
}
