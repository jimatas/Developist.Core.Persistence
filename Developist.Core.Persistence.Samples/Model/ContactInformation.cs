// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence.Samples
{
    public class ContactInformation
    {
        public string Email { get; set; }
        public string HomeTelephone { get; set; }
        public Address HomeAddress { get; set; } = new();
    }
}
