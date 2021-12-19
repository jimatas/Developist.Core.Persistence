// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Developist.Core.Persistence.Entities;
using Developist.Core.Utilities;

using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests.Integration.Fixture
{
    public static class RepositoryExtensions
    {
        public static void AddRange<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            Ensure.Argument.NotNull(entities, nameof(entities));

            foreach (TEntity entity in entities)
            {
                repository.Add(entity);
            }
        }
    }
}
