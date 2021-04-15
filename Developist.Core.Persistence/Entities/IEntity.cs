// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Defines an entity that is retrieved and persisted through a repository.
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// Determines this entity's persistent state.
        /// </summary>
        /// <value><see langword="true"/> if this entity has not yet been persisted, <see langword="false"/> otherwise.</value>
        bool IsTransient { get; }
    }
}
