// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Samples
{
    public class Person : EntityBase<int>
    {
        public Person() { }
        public Person(int id) : base(id) { }

        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public ContactInformation Contact { get; set; } = new();
    }
}
