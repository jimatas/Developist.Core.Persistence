// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

namespace Developist.Core.Persistence.EntityFrameworkCore
{
    public interface IUnitOfWork<TDbContext> : IUnitOfWork
        where TDbContext : DbContext
    {
        TDbContext DbContext { get; }
    }
}
