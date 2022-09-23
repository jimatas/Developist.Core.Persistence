using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }
}
