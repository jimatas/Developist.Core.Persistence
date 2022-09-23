using System;

namespace Developist.Core.Persistence
{
    public class UnitOfWorkCompletedEventArgs : EventArgs
    {
        public UnitOfWorkCompletedEventArgs(IUnitOfWork unitOfWork)
            => UnitOfWork = unitOfWork;

        public IUnitOfWork UnitOfWork { get; }
    }
}
