// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Collections.Generic;

namespace Developist.Core.Persistence.Tests
{
    public static class RepositoryExtensions
    {
        public static void AddRange<TEntity>(this IRepository<TEntity> repository, IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            if (entities is null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (TEntity entity in entities)
            {
                repository.Add(entity);
            }
        }
    }
}
