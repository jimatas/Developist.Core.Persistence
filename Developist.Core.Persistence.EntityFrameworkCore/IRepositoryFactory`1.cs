using Developist.Core.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public interface IRepositoryFactory<TDbContext> : IRepositoryFactory
        where TDbContext : DbContext
    {
        IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> unitOfWork)
            where TEntity : class, IEntity;
    }
}
