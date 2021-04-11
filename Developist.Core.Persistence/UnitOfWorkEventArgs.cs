// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;

namespace Developist.Core.Persistence
{
    public class UnitOfWorkEventArgs : EventArgs
    {
        public UnitOfWorkEventArgs(IUnitOfWork uow) => UnitOfWork = uow ?? throw new ArgumentNullException(nameof(uow));
        public IUnitOfWork UnitOfWork { get; }
    }
}
