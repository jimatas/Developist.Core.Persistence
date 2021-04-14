// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests
{
    public class Message : EntityBase<int>
    {
        public Message() { }
        public Message(int id) : base(id) { }

        public string Text { get; set; }

        public Person Sender { get; set; }
        public ICollection<Person> Recipients { get; set; } = new HashSet<Person>();
    }
}
