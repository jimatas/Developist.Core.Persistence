// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// The event data that will be passed in to the handlers when the <see cref="IUnitOfWork.Completed"/> event is fired.
    /// </summary>
    public class UnitOfWorkCompletedEventArgs : EventArgs
    {
        public UnitOfWorkCompletedEventArgs(IUnitOfWork uow) => UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
        
        /// <summary>
        /// The unit of work that was completed.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; }
    }
}
