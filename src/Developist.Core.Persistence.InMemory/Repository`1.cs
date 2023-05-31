using Developist.Core.Persistence.Filtering;
using Developist.Core.Persistence.IncludePaths;
using Developist.Core.Persistence.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    /// <summary>
    /// Represents an in-memory repository for a specified entity type.
    /// </summary>
    /// <typeparam name="T">The type of entity being managed by the repository.</typeparam>
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class with the specified unit of work.
        /// </summary>
        /// <param name="unitOfWork">The unit of work associated with the repository.</param>
        public Repository(IUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            UnitOfWork.Completed += UnitOfWorkCompleted;
        }

        /// <summary>
        /// Gets the in-memory data store for the repository.
        /// </summary>
        public ISet<T> DataStore { get; private set; } = new HashSet<T>();

        private ISet<T> WorkingDataStore { get; } = new HashSet<T>();

        /// <inheritdoc/>
        public IUnitOfWork UnitOfWork { get; }

        /// <inheritdoc/>
        public void Add(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            WorkingDataStore.Add(entity);
        }

        /// <inheritdoc/>
        public void Remove(T entity)
        {
            if (entity is null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            WorkingDataStore.Remove(entity);
        }

        /// <inheritdoc/>
        public Task<int> CountAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(DataStore.Count);
        }

        /// <inheritdoc/>
        public Task<int> CountAsync(IFilterCriteria<T> criteria, CancellationToken cancellationToken = default)
        {
            var query = DataStore.AsQueryable().FilterBy(criteria);

            return Task.FromResult(query.Count());
        }

        /// <inheritdoc/>
        public Task<IPaginatedList<T>> ListAsync(IPaginator<T> paginator, CancellationToken cancellationToken = default)
        {
            return DataStore.AsQueryable().ToPaginatedListAsync(paginator, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IPaginatedList<T>> ListAsync(IPaginator<T> paginator, IIncludePathsBuilder<T> includePaths, CancellationToken cancellationToken = default)
        {
            return ListAsync(paginator, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IPaginatedList<T>> FindAsync(IFilterCriteria<T> criteria, IPaginator<T> paginator, CancellationToken cancellationToken = default)
        {
            var query = DataStore.AsQueryable().FilterBy(criteria);

            return query.ToPaginatedListAsync(paginator, cancellationToken);
        }

        /// <inheritdoc/>
        public Task<IPaginatedList<T>> FindAsync(IFilterCriteria<T> criteria, IPaginator<T> paginator, IIncludePathsBuilder<T> includePaths, CancellationToken cancellationToken = default)
        {
            return FindAsync(criteria, paginator, cancellationToken);
        }

        private void UnitOfWorkCompleted(object sender, UnitOfWorkCompletedEventArgs e)
        {
            DataStore = new HashSet<T>(WorkingDataStore);
        }
    }
}
