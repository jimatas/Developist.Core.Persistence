// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Supports the deferred creation of a unit of work.
    /// </summary>
    /// <remarks>
    /// Consider injecting this interface into consumers of the unit of work dependency instead of injecting the unit of work directly.
    /// </remarks>
    public interface IUnitOfWorkManager
    {
        /// <summary>
        /// Starts a new unit of work.
        /// </summary>
        /// <param name="transactional">Indicates whether the unit of work will be operating within the scope of an explicit data store transaction, if supported by the data provider.</param>
        /// <returns>The unit of work that was started.</returns>
        IUnitOfWork StartNew(bool transactional = false);
    }
}
