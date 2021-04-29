// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    internal sealed class SemaphoreLocker
    {
        private readonly SemaphoreSlim semaphore = new(initialCount: 1, maxCount: 1);

        public void Lock(Action action)
        {
            semaphore.Wait();
            try
            {
                action();
            }
            finally
            {
                semaphore.Release();
            }
        }

        public async Task LockAsync(Func<Task> action, CancellationToken cancellationToken = default)
        {
            await semaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                // Safe to release. If WaitAsync completed successfully, the semaphore will have acquired the lock.
                // If it threw a (TaskCanceled)Exception, we wouldn't even be here.
                semaphore.Release();
            }
        }
    }
}
