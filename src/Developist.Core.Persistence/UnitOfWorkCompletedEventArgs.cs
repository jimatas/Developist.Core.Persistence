using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence;

/// <summary>
/// Provides data for the <see cref="IUnitOfWorkBase.Completed"/> event.
/// </summary>
public class UnitOfWorkCompletedEventArgs : EventArgs
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnitOfWorkCompletedEventArgs"/> class.
    /// </summary>
    /// <param name="unitOfWork">The unit of work that completed.</param>
    public UnitOfWorkCompletedEventArgs(IUnitOfWorkBase unitOfWork)
    {
        UnitOfWork = Ensure.NotNull(unitOfWork);
    }

    /// <summary>
    /// Gets the unit of work that completed.
    /// </summary>
    public IUnitOfWorkBase UnitOfWork { get; }
}
