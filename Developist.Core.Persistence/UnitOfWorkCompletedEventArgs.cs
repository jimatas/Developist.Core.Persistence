// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

using System;

namespace Developist.Core.Persistence
{
    public class UnitOfWorkCompletedEventArgs : EventArgs
    {
        public UnitOfWorkCompletedEventArgs(IUnitOfWork uow)
            => UnitOfWork = Ensure.Argument.NotNull(uow, nameof(uow));

        public IUnitOfWork UnitOfWork { get; }
    }
}
