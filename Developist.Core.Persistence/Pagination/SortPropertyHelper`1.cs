// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Developist.Core.Persistence.Pagination
{
    internal static class SortPropertyHelper<T>
    {
        public static IEnumerable<SortProperty<T>> ParseFromString(string value)
        {
            Ensure.Argument.NotNullOrWhiteSpace(value, nameof(value));

            foreach (var directive in value.Split(',').Where(s => !string.IsNullOrWhiteSpace(s)).Select(s => s.Trim()))
            {
                var propertyName = directive;

                bool descending;
                if ((descending = directive.StartsWith("-")) || directive.StartsWith("+"))
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
