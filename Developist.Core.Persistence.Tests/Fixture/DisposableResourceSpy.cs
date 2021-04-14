// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    public class DisposableResourceSpy : DisposableBase
    {
        public int ReleaseManagedResourcesCallCount { get; private set; }
        public int ReleaseNativeResourcesCallCount { get; private set; }
        public int ReleaseManagedResourcesAsyncCallCount { get; private set; }

        protected override void ReleaseManagedResources()
        {
            ReleaseManagedResourcesCallCount++;
            base.ReleaseManagedResources();
        }

        protected override void ReleaseNativeResources()
        {
            ReleaseNativeResourcesCallCount++;
            base.ReleaseNativeResources();
        }

        protected override ValueTask ReleaseManagedResourcesAsync()
        {
            ReleaseManagedResourcesAsyncCallCount++;
            return base.ReleaseManagedResourcesAsync();
        }
    }
}
