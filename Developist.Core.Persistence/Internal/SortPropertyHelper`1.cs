// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections.Generic;

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

            foreach (var directive in value.Split(',', StringSplitOptions.TrimEntries))
            {
                var propertyName = directive;

                bool descending;
                if ((descending = directive.StartsWith('-')) || directive.StartsWith('+')) // Also handle possible +-prefix.
                {
                    propertyName = directive.Substring(1).TrimStart('(').TrimEnd(')');
                }

                var direction = descending ? SortDirection.Descending : SortDirection.Ascending;

                SortProperty<T> property;
                try
                {
                    property = new SortProperty<T>(propertyName, direction);
                }
                catch (ArgumentException exception)
                {
                    throw new FormatException("At least one of the sorting directives in the input string could not be parsed successfully.", exception);
                }

                yield return property;
            }
        }
    }
}
