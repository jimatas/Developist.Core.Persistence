// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Developist.Core.Persistence.EntityFramework
{
    internal static class DbContextExtensions
    {
        public static EntityEntry Entry<TEntity>(this DbContext dbContext, TEntity entity, bool attachIfDetached) where TEntity : class
        {
            var entry = dbContext.Entry(entity);
            if (entry.State == EntityState.Detached && attachIfDetached)
            {
                dbContext.Set<TEntity>().Attach(entity);
            }
            return entry;
        }

        public static void ValidateChangedEntities(this DbContext dbContext)
        {
            static bool hasChanged(EntityEntry entry) => entry.State == EntityState.Added || entry.State == EntityState.Modified;

            var exceptions = new List<Exception>();

            foreach (var entity in dbContext.ChangeTracker.Entries().Where(hasChanged).Select(entry => entry.Entity))
            {
                var context = new ValidationContext(entity, serviceProvider: null, items: null);
                var errors = new List<ValidationResult>();

                if (!Validator.TryValidateObject(entity, context, errors, validateAllProperties: true))
                {
                    exceptions.AddRange(errors.Select(error => new ValidationException(error.ErrorMessage)));
                }
            }

            if (exceptions.Any())
            {
                throw exceptions.Count == 1
                    ? exceptions.Single() 
                    : new AggregateException(exceptions);
            }
        }
    }
}
