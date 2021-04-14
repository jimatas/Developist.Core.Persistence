// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests
{
    public class Book : EntityBase<int>
    {
        public Book() { }
        public Book(int id) => Id = id;

        public string Title { get; set; }
        public ICollection<Person> Authors { get; set; }
        public Genre Genre { get; set; }
    }
}
