// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System;
using System.Threading.Tasks;

namespace Developist.Core.Persistence
{
    /// <summary>
    /// Provides the boiler plate code for implementing the dispose/async dispose pattern.
    /// </summary>
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
        /// <summary>
        /// Override to release any managed resources held by this instance.
        /// </summary>
        protected virtual void ReleaseManagedResources() { }

        /// <summary>
        /// Override to release any native (unmanaged) resources held by this instance.
        /// </summary>
        protected virtual void ReleaseNativeResources() { }

        /// <summary>
        /// Override to asynchronously release any managed resources held by this instance.
        /// </summary>
        /// <returns>An awaitable value task representing the asynchronous operation.</returns>
        protected virtual ValueTask ReleaseManagedResourcesAsync() => default;
        #endregion
    }
}
