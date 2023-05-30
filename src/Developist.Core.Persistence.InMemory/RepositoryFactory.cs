using Microsoft.Extensions.DependencyInjection;
using System;

namespace Developist.Core.Persistence.InMemory
{
    /// <summary>
    /// Represents a repository factory that creates in-memory repositories for the specified entity types.
    /// </summary>
    public class RepositoryFactory : IRepositoryFactory
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryFactory"/> class with the specified service provider.
        /// </summary>
        /// <param name="serviceProvider">The service provider to use when creating repository instances.</param>
        public RepositoryFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        /// <inheritdoc/>
        public IRepository<T> Create<T>(IUnitOfWork unitOfWork) where T : class
        {
            var repositoryType = GetRepositoryImplementationType<T>();
            var repository = (IRepository<T>)ActivatorUtilities.CreateInstance(_serviceProvider, repositoryType, unitOfWork);

            return repository;
        }

        /// <summary>
        /// Gets the repository implementation type for the specified entity type.
        /// </summary>
        /// <remarks>
        /// By default, this method returns the type of the <see cref="Repository{T}"/> class.
        /// Subclasses can override this method to provide their own implementation of the <see cref="IRepository{T}"/> interface.
        /// </remarks>
        /// <typeparam name="T">The entity type for which to get the repository implementation type.</typeparam>
        /// <returns>The repository implementation type for the specified entity type.</returns>
        protected virtual Type GetRepositoryImplementationType<T>() where T : class
        {
            return typeof(Repository<T>);
        }
    }
}
