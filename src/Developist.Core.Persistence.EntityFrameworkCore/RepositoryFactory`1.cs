using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Implements the <see cref="IRepositoryFactory"/> interface to provide instances of <see cref="IRepository{T}"/> objects that are specific to Entity Framework Core.
/// </summary>
/// <typeparam name="TContext">The type of the data context.</typeparam>
public class RepositoryFactory<TContext> : IRepositoryFactory<TContext> where TContext : DbContext
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryFactory{TContext}"/> class with the specified service provider.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve dependencies.</param>
    public RepositoryFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc/>
    public IRepository<T> Create<T>(IUnitOfWork unitOfWork) where T : class
    {
        return Create<T>((IUnitOfWork<TContext>)unitOfWork);
    }

    /// <inheritdoc/>
    public IRepository<T> Create<T>(IUnitOfWork<TContext> unitOfWork) where T : class
    {
        var repositoryType = GetRepositoryImplementationType<T>();
        var repository = (IRepository<T>)ActivatorUtilities.CreateInstance(_serviceProvider, repositoryType, unitOfWork);
        return repository;
    }

    /// <summary>
    /// Gets the type of repository implementation to use for entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <remarks>
    /// By default, this method returns the type of the <see cref="Repository{T, TContext}"/> class.
    /// Subclasses can override this method to provide their own implementation of the <see cref="IRepository{T}"/> interface.
    /// </remarks>
    /// <typeparam name="T">The type of entity for which to get the repository implementation.</typeparam>
    /// <returns>The type of repository implementation to use for entities of type <typeparamref name="T"/>.</returns>
    protected virtual Type GetRepositoryImplementationType<T>() where T : class
    {
        return typeof(Repository<T, TContext>);
    }
}
