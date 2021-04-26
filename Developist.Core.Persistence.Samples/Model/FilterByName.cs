// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Samples
{
    public class FilterByName : IQueryableFilter<Person>
    {
        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        /// <summary>
        /// If true, the name(s) will be compared using Contains instead of Equals.
        /// </summary>
        public bool UsePartialMatching { get; set; }

        public IQueryable<Person> Filter(IQueryable<Person> sequence)
        {
            Expression<Func<Person, bool>> predicate = p => true;

            if (GivenName is not null)
            {
                predicate = predicate.AndAlso(
                    p => UsePartialMatching
                        ? p.GivenName.Contains(GivenName)
                        : p.GivenName.Equals(GivenName));
            }

            if (FamilyName is not null)
            {
                predicate = predicate.AndAlso(
                    p => UsePartialMatching
                        ? p.FamilyName.Contains(FamilyName)
                        : p.FamilyName.Equals(FamilyName));
            }

            return sequence.Where(predicate);
        }
    }
}
