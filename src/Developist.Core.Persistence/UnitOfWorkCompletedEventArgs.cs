using System;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Provides data for the <see cref="IUnitOfWork.Completed"/> event.
    /// </summary>
    public class UnitOfWorkCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWorkCompletedEventArgs"/> class.
        /// </summary>
        /// <param name="unitOfWork">The unit of work that completed.</param>
        public UnitOfWorkCompletedEventArgs(IUnitOfWork unitOfWork)
            => UnitOfWork = unitOfWork;

        /// <summary>
        /// Gets the unit of work that completed.
        /// </summary>
        public IUnitOfWork UnitOfWork { get; }
    }
}
