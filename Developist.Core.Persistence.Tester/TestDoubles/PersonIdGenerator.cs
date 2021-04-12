// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;

namespace Developist.Core.Persistence.Tester
{
    public class PersonIdGenerator : IIdGenerator<Person, int>
    {
        public static readonly PersonIdGenerator Default = new();
        private static int idCounter;

        public void Initialize(int id = 0) => Interlocked.Exchange(ref idCounter, id);
        public int GenerateId() => Interlocked.Increment(ref idCounter);
    }
}
