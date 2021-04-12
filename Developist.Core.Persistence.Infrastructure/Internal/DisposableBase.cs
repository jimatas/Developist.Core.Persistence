// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    public class DisposableBase : IDisposable, IAsyncDisposable
    {
        ~DisposableBase()
        {
            Dispose(disposing: false);
        }

        public bool IsDisposed { get; protected set; }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async ValueTask DisposeAsync()
        {
            await DisposeAsyncCore().ConfigureAwait(false);
            Dispose(disposing: false);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (disposing)
            {
                ReleaseManagedResources();
            }
            ReleaseNativeResources();

            IsDisposed = true;
        }

        protected virtual async ValueTask DisposeAsyncCore()
        {
            if (IsDisposed)
            {
                return;
            }

            await ReleaseManagedResourcesAsync().ConfigureAwait(false);

            IsDisposed = true;
        }

        #region Overridable hooks
        protected virtual void ReleaseManagedResources() { }
        protected virtual void ReleaseNativeResources() { }
        protected virtual ValueTask ReleaseManagedResourcesAsync() => default;
        #endregion
    }
}
