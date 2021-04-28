// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.EntityFrameworkCore;

using System;

namespace Developist.Core.Persistence.EntityFramework
{
    public class UnitOfWorkManager<TDbContext> : UnitOfWorkManager, IUnitOfWorkManager where TDbContext : DbContext
    {
        public UnitOfWorkManager(IServiceProvider serviceProvider) : base(serviceProvider) { }

        public override IUnitOfWork StartNew(bool transactional = true)
        {
            var uow = base.StartNew();
            if (transactional)
            {
                (uow as IUnitOfWork<TDbContext>)?.EnsureTransactional();
            }
            return uow;
        }
    }
}
