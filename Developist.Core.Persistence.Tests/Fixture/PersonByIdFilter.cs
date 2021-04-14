// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Linq;

namespace Developist.Core.Persistence.Tests
{
    public class PersonByIdFilter : IQueryableFilter<Person>
    {
        private readonly int id;
        public PersonByIdFilter(int id) => this.id = id;
        public IQueryable<Person> Filter(IQueryable<Person> sequence) => sequence.Where(p => p.Id == id);
    }
}
