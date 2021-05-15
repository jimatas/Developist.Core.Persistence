// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence
{
    internal static class SortPropertyHelper<T>
    {
        /// <summary>
        /// Parses a string such as <c>"-DateOfBirth,FamilyName"</c> to an enumerable collection of <see cref="SortProperty{T}"/> objects.
        /// </summary>
        /// <param name="value">A string containing potentially multiple sorting directives.</param>
        /// <returns>An enumerable collection of <see cref="SortProperty{T}"/> objects parsed from the input string.</returns>
        public static IEnumerable<SortProperty<T>> ParseFromString(string value)
        {
            foreach (var directive in value.Split(',').Select(s => s.Trim()).Where(s => s != string.Empty))
            {
                var property = directive;

                bool descending;
                if ((descending = property.StartsWith('-')) || property.StartsWith('+')) // Also handle possible +-prefix.
                {
                    property = property.Substring(1).TrimStart('(').TrimEnd(')');
                }

                if (property == string.Empty)
                {
                    throw new FormatException("At least one of the sorting directives contained in the supplied string could not be parsed successfully.");
                }

                var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                yield return new SortProperty<T>(property, direction);
            }
        }
    }
}
