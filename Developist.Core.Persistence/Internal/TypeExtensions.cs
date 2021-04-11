// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Reflection;

namespace Developist.Core.Persistence
{
    internal static class TypeExtensions
    {
        public static PropertyInfo GetPublicProperty(this Type type, string propertyName)
        {
            const BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance;

            if (type.IsInterface)
            {
                return type.GetProperty(propertyName, bindingAttr);
            }

            return new[] { type }.Concat(type.GetInterfaces()).Select(iface => iface.GetProperty(propertyName, bindingAttr)).FirstOrDefault();
        }
    }
}
