// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests.Integration.Fixture
{
    public class Person : EntityBase<int>
    {
        public Person() { }
        public Person(int id) : base(id) { }

        public string GivenName { get; set; }
        public string FamilyName { get; set; }

        public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();
        public ICollection<Message> ReceivedMessages { get; set; } = new HashSet<Message>();
    }
}
