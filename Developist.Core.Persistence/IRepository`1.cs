using Developist.Core.Persistence.Entities;

namespace Developist.Core.Persistence
{
    public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
        where TEntity : IEntity
    {
        IUnitOfWork UnitOfWork { get; }

        void Add(TEntity entity);
        void Remove(TEntity entity);
    }
}
