// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Developist.Core.Persistence.Tests
{
    public class PersonByNameFilter : IQueryableFilter<Person>
    {
        private readonly bool andAlso;
        public PersonByNameFilter(bool andAlso = true) => this.andAlso = andAlso;

        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public IQueryable<Person> Filter(IQueryable<Person> sequence)
        {
            Expression<Func<Person, bool>> predicate = p => andAlso;

            if (!string.IsNullOrEmpty(GivenName))
            {
                if (andAlso)
                {
                    predicate = predicate.AndAlso(p => p.GivenName.Equals(GivenName));
                }
                else
                {
                    predicate = predicate.OrElse(p => p.GivenName.Equals(GivenName));
                }
            }

            if (!string.IsNullOrEmpty(FamilyName))
            {
                if (andAlso)
                {
                    predicate = predicate.AndAlso(p => p.FamilyName.Equals(FamilyName));
                }
                else
                {
                    predicate = predicate.OrElse(p => p.FamilyName.Equals(FamilyName));
                }
            }

            return sequence.Where(predicate);
        }
    }
}
