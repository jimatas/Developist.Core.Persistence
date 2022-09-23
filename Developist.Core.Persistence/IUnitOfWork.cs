using Developist.Core.Persistence.Entities;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public interface IUnitOfWork : IAsyncDisposable
    {
        event EventHandler<UnitOfWorkCompletedEventArgs>? Completed;
        Task CompleteAsync(CancellationToken cancellationToken = default);

        bool IsTransactional { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        IRepository<TEntity> Repository<TEntity>() 
            where TEntity : class, IEntity;
    }
}
