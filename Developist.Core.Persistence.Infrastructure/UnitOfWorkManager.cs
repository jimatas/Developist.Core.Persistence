// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.Extensions.DependencyInjection;

using System;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Default implementation of the <see cref="IUnitOfWorkManager"/> interface that resolves the unit of work dependency using the <see cref="IServiceProvider"/> that is passed in through the constructor.
    /// </summary>
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private readonly IServiceProvider serviceProvider;

        public UnitOfWorkManager(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual IUnitOfWork StartNew(bool transactional = false)
        {
            return serviceProvider.GetRequiredService<IUnitOfWork>();
        }
    }
}
