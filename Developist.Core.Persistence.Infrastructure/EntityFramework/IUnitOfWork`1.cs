// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFramework
{
    /// <summary>
    /// Extends the <see cref="IUnitOfWork"/> interface with a generic DbContext type parameter
    /// </summary>
    /// <typeparam name="TDbContext">The type of the DbContext uderlying this unit of work.</typeparam>
    public interface IUnitOfWork<TDbContext> : IUnitOfWork where TDbContext : DbContext
    {
        /// <summary>
        /// This unit of work's underlying DbContext.
        /// </summary>
        TDbContext DbContext { get; }
    }
}
