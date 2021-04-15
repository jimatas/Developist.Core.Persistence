// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Extends the <see cref="IEntity"/> interface adding an Id property through which the entity is uniquely identified.
    /// </summary>
    /// <typeparam name="TIdentifier">The type of the identifier.</typeparam>
    public interface IEntity<TIdentifier> : IEntity where TIdentifier : IEquatable<TIdentifier>
    {
        /// <summary>
        /// The entity's unique identifier. 
        /// Typically the primary key of the record in which the entity is stored in the database.
        /// </summary>
        TIdentifier Id { get; }
    }
}
