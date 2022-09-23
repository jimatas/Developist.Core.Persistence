using Developist.Core.Persistence.Entities;

namespace Developist.Core.Persistence
{
    public interface IRepositoryFactory
    {
        IRepository<TEntity> Create<TEntity>(IUnitOfWork unitOfWork)
            where TEntity : class, IEntity;
    }
}
