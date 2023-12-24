using Developist.Core.Persistence.Utilities;

namespace Developist.Core.Persistence.EntityFrameworkCore;

/// <summary>
/// Provides extension methods for <see cref="IRepository{T}"/> to enhance its functionality with additional query capabilities, 
/// such as including related entities and disabling tracking.
/// </summary>
public static partial class RepositoryExtensions
{
    /// <summary>
    /// Extends the repository with a specified feature using a given query extender.
    /// </summary>
    /// <typeparam name="T">The entity type of the repository.</typeparam>
    /// <param name="repository">The repository to be extended.</param>
    /// <param name="queryExtender">The query extender to be applied to the repository.</param>
    /// <param name="featureName">The name of the feature to be enabled or disabled in the repository.</param>
    /// <returns>A new <see cref="IRepository{T}"/> instance that includes the specified feature.</returns>
    public static IRepository<T> ExtendWithFeature<T>(
        this IRepository<T> repository,
        IQueryExtender<T> queryExtender,
        string featureName) where T : class
    {
        Ensure.NotNull(queryExtender);
        Ensure.NotNullOrWhiteSpace(featureName);

        var efCoreRepository = EnsureRepositorySupportsFeature(repository, featureName);

        return new ExtendableQueryRepository<T>(
            unitOfWork: efCoreRepository.UnitOfWork,
            queryExtender: CreateCompositeExtenderIfNeeded(efCoreRepository, queryExtender));
    }

    private static Repository<T> EnsureRepositorySupportsFeature<T>(
        IRepository<T> repository,
        string featureName) where T : class
    {
        return repository as Repository<T>
            ?? throw new NotSupportedException($"The repository of type '{repository.GetType().Name}' does not have support for {featureName}.");
    }

    private static IQueryExtender<T> CreateCompositeExtenderIfNeeded<T>(
        Repository<T> repository,
        IQueryExtender<T> queryExtender) where T : class
    {
        if (repository is ExtendableQueryRepository<T> { QueryExtender: { } existingQueryExtender })
        {
            return new CompositeQueryExtender<T>(existingQueryExtender, queryExtender);
        }

        return queryExtender;
    }

    // Declared internal for unit test visibility.
    internal class CompositeQueryExtender<T> : IQueryExtender<T>
    {
        private readonly List<IQueryExtender<T>> _queryExtenders = new();

        public CompositeQueryExtender(params IQueryExtender<T>[] queryExtenders)
        {
            foreach (var queryExtender in queryExtenders)
            {
                if (queryExtender is CompositeQueryExtender<T> compositeQueryExtender)
                {
                    _queryExtenders.AddRange(compositeQueryExtender._queryExtenders);
                }
                else
                {
                    _queryExtenders.Add(queryExtender);
                }
            }
        }

        public IQueryable<T> Extend(IQueryable<T> query)
        {
            foreach (var queryExtender in _queryExtenders)
            {
                query = queryExtender.Extend(query);
            }

            return query;
        }
    }
}
