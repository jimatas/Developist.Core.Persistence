// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence.Entities
{
    public interface IEntity<TIdentifier> : IEntity
        where TIdentifier : IEquatable<TIdentifier>
    {
        TIdentifier Id { get; }
    }
}
