// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence.InMemory
{
    public class UnitOfWork : UnitOfWorkBase
    {
        public UnitOfWork() : this(new RepositoryFactory()) { }
        public UnitOfWork(IRepositoryFactory repositoryFactory) : base(repositoryFactory) { }

        public override bool IsTransactional => false;

        public override void BeginTransaction() { }

        public override void Complete()
            => OnCompleted(new UnitOfWorkCompletedEventArgs(this));

        public override Task CompleteAsync(CancellationToken cancellationToken = default)
        {
            Complete();
            return Task.CompletedTask;
        }
    }
}
