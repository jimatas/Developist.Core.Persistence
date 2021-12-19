// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace Developist.Core.Persistence.EntityFramework
{
    internal static class DbContextExtensions
    {
        public static DbEntityEntry Entry<TEntity>(this DbContext dbContext, TEntity entity, bool attachIfDetached)
            where TEntity : class
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached && attachIfDetached)
            {
                dbContext.Set<TEntity>().Attach(entity);
            }
            return entry;
        }
    }
}
