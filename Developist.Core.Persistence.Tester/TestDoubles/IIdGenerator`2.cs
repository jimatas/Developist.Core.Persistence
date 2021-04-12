// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence.Tester
{
    public interface IIdGenerator<TEntity, TIdentifier>
    where TEntity : IEntity<TIdentifier>
    where TIdentifier : IEquatable<TIdentifier>
    {
        void Initialize(TIdentifier id = default);
        TIdentifier GenerateId();
    }
}
