using Developist.Core.Persistence.Entities;

using System;

namespace Developist.Core.Persistence.InMemory
{
    public class RepositoryFactory : IRepositoryFactory
    {
        public IRepository<TEntity> Create<TEntity>(IUnitOfWork unitOfWork)
            where TEntity : class, IEntity
        {
            var repositoryType = GetRepositoryImplementationType<TEntity>();
            var repository = (IRepository<TEntity>)Activator.CreateInstance(repositoryType, new[] { unitOfWork });
            return repository;
        }

        protected virtual Type GetRepositoryImplementationType<TEntity>()
            where TEntity : IEntity
        {
            return typeof(Repository<TEntity>);
        }
    }
}
