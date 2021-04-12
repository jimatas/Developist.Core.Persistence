// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Tests
{
    public static class PersonExtensions
    {
        public static string FullName(this Person p) => $"{p.GivenName} {p.FamilyName}";
    }
}
