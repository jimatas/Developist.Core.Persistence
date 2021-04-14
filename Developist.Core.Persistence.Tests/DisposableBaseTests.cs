// Copyright (c) 2021 Jim Atas. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for details.

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Threading.Tasks;

namespace Developist.Core.Persistence.Tests
{
    [TestClass]
    public class DisposableBaseTests
    {
        [TestMethod]
        public void NewInstance_ByDefault_IsNotDisposed()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act

            // Assert
            Assert.IsFalse(resource.IsDisposed);
        }

        [TestMethod]
        public void Dispose_ByDefault_Disposes()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act
            resource.Dispose();

            // Assert
            Assert.IsTrue(resource.IsDisposed);
        }

        [TestMethod]
        public void Dispose_ByDefault_CallsAllSyncHooks()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act
            resource.Dispose();

            // Assert
            Assert.AreEqual(1, resource.ReleaseManagedResourcesCallCount);
            Assert.AreEqual(1, resource.ReleaseNativeResourcesCallCount);
            Assert.AreEqual(0, resource.ReleaseManagedResourcesAsyncCallCount);
        }

        [TestMethod]
        public void Dispose_CalledTwice_CallsSyncHooksOnlyOnce()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act
            resource.Dispose();
            resource.Dispose();

            // Assert
            Assert.AreEqual(1, resource.ReleaseManagedResourcesCallCount);
            Assert.AreEqual(1, resource.ReleaseNativeResourcesCallCount);
            Assert.AreEqual(0, resource.ReleaseManagedResourcesAsyncCallCount);
        }

        [TestMethod]
        public async Task DisposeAsync_ByDefault_CallsAsyncHook()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act
            await resource.DisposeAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, resource.ReleaseManagedResourcesCallCount);
            Assert.AreEqual(0, resource.ReleaseNativeResourcesCallCount);
            Assert.AreEqual(1, resource.ReleaseManagedResourcesAsyncCallCount);
        }

        [TestMethod]
        public async Task DisposeAsync_CalledTwice_CallsAsyncHookOnlyOnce()
        {
            // Arrange
            var resource = new DisposableResourceSpy();

            // Act
            await resource.DisposeAsync().ConfigureAwait(false);
            await resource.DisposeAsync().ConfigureAwait(false);

            // Assert
            Assert.AreEqual(0, resource.ReleaseManagedResourcesCallCount);
            Assert.AreEqual(0, resource.ReleaseNativeResourcesCallCount);
            Assert.AreEqual(1, resource.ReleaseManagedResourcesAsyncCallCount);
        }
    }
}
