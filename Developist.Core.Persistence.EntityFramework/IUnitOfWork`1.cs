// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Data.Entity;

namespace Developist.Core.Persistence.EntityFramework
{
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }
}
