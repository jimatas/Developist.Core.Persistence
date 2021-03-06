// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Pagination
{
    public class SortProperty<T, TProperty> : SortProperty<T>
    {
        public SortProperty(Expression<Func<T, TProperty>> property, SortDirection direction) 
            : base(direction)
        {
            Property = Ensure.Argument.NotNull(property, nameof(property));
        }

        public new Expression<Func<T, TProperty>> Property { get; }

        public override IOrderedQueryable<T> ApplyTo(IQueryable<T> sequence)
        {
            return sequence.IsOrdered()
                ? Direction == SortDirection.Ascending
                    ? ((IOrderedQueryable<T>)sequence).ThenBy(Property)
                    : ((IOrderedQueryable<T>)sequence).ThenByDescending(Property)
                : Direction == SortDirection.Ascending
                    ? sequence.OrderBy(Property)
                    : sequence.OrderByDescending(Property);
        }
    }
}
