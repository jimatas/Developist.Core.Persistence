using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class UnitOfWork : UnitOfWorkBase
    {
        public UnitOfWork(IRepositoryFactory? repositoryFactory = null)
            : base(repositoryFactory ?? new RepositoryFactory()) { }

        public override bool IsTransactional => false;

        public override Task BeginTransactionAsync(CancellationToken cancellationToken = default) => Task.CompletedTask;

        public override Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            OnCompleted(new UnitOfWorkCompletedEventArgs(this));
            return Task.CompletedTask;
        }
    }
}
