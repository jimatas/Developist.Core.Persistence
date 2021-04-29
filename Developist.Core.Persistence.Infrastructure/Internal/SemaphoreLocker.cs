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
            var waitTask = semaphore.WaitAsync(cancellationToken);
            await waitTask.ConfigureAwait(false);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                if (waitTask.IsCompletedSuccessfully || (waitTask.IsFaulted && !waitTask.IsCanceled))
                {
                    semaphore.Release();
                }
            }
        }
    }
}
