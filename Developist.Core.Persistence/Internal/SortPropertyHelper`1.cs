// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence
{
    internal static class SortPropertyHelper<T>
    {
        /// <summary>
        /// Parses a string such as <c>"-DateOfBirth,FamilyName"</c> to the corresponding collection of <see cref="SortProperty{T}"/> objects.
        /// </summary>
        /// <param name="value">A string containing one or more sorting directives.</param>
        /// <returns>An enumerable collection of <see cref="SortProperty{T}"/> objects parsed from the input string.</returns>
        public static IEnumerable<SortProperty<T>> ParseFromString(string value)
        {
            Ensure.Argument.NotNullOrWhiteSpace(value, nameof(value));

            foreach (var directive in value.Split(',').Select(s => s.Trim()).Where(s => s != string.Empty))
            {
                var property = directive;

                bool descending;
                if ((descending = property.StartsWith('-')) || property.StartsWith('+')) // Also handle possible +-prefix.
                {
                    property = property.Substring(1).TrimStart('(').TrimEnd(')');
                }

                if (property.Trim() == string.Empty)
                {
                    throw new FormatException("At least one of the sorting directives in the input string could not be parsed successfully.");
                }

                var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                yield return new SortProperty<T>(property, direction);
            }
        }
    }
}
