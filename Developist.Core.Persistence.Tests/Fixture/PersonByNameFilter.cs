// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence.Tests
{
    public class PersonByNameFilter : IQueryableFilter<Person>
    {
        public bool UsePartialMatching { get; set; }

        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public IQueryable<Person> Filter(IQueryable<Person> sequence)
        {
            if (!string.IsNullOrEmpty(GivenName))
            {
                sequence = sequence.Where(p => UsePartialMatching ? p.GivenName.Contains(GivenName) : p.GivenName.Equals(GivenName));
            }

            if (!string.IsNullOrEmpty(FamilyName))
            {
                sequence = sequence.Where(p => UsePartialMatching ? p.FamilyName.Contains(FamilyName) : p.FamilyName.Equals(FamilyName));
            }

            return sequence;
        }
    }
}
