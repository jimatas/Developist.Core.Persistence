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

        public async Task LockAsync(Func<Task> action)
        {
            await semaphore.WaitAsync().ConfigureAwait(false);
            try
            {
                await action().ConfigureAwait(false);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }
}
