// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Utilities;

namespace Developist.Core.Persistence
{
    internal class RepositoryWrapper
    {
        private readonly object repository;
        public RepositoryWrapper(object repository) => this.repository = Ensure.Argument.NotNull(repository, nameof(repository));
        public IRepository<TEntity> Repository<TEntity>() where TEntity : class, IEntity => (IRepository<TEntity>)repository;
    }
}
