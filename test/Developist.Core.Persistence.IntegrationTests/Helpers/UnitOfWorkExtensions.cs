using Developist.Core.Persistence.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.IntegrationTests.Helpers;

internal static class UnitOfWorkExtensions
{
    public static DisposableUnitOfWorkWrapper<TContext> AsDisposableUnitOfWork<TContext>(
        this IUnitOfWork<TContext> unitOfWork,
        Func<DisposableUnitOfWorkWrapper<TContext>, ValueTask>? disposeAction = default) where TContext : DbContext
    {
        return new DisposableUnitOfWorkWrapper<TContext>(unitOfWork, disposeAction);
    }
}
