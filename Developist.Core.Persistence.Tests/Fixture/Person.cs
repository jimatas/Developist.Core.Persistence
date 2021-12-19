// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests
{
    public class Person : EntityBase<int>
    {
        public Person() { }
        public Person(int id) => Id = id;

        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public int? Age { get; set; }
        public ICollection<Person> Friends { get; set; }
        public Book FavoriteBook { get; set; }
    }
}
