using Developist.Core.Persistence.Entities;

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public class RepositoryFactory<TDbContext> : IRepositoryFactory<TDbContext>
        where TDbContext : DbContext
    {
        IRepository<TEntity> IRepositoryFactory.Create<TEntity>(IUnitOfWork unitOfWork)
        {
            return Create<TEntity>((IUnitOfWork<TDbContext>)unitOfWork);
        }

        public virtual IRepository<TEntity> Create<TEntity>(IUnitOfWork<TDbContext> unitOfWork)
            where TEntity : class, IEntity
        {
            var repositoryType = GetRepositoryImplementationType<TEntity>();
            var repository = (IRepository<TEntity>)Activator.CreateInstance(repositoryType, new[] { unitOfWork })!;
            return repository;
        }

        protected virtual Type GetRepositoryImplementationType<TEntity>()
            where TEntity : class, IEntity
        {
            return typeof(Repository<TEntity, TDbContext>);
        }
    }
}
