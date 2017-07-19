using Hull.Unity.Pooling;
using NUnit.Framework;
using UnityEngine;

namespace Hull.Unity.Editor.Tests.Pooling {
    [TestFixture]
    public class PoolTest {
        private static bool instantiateCalled;
        private static bool pooledCalled;

        private class PoolableTest : MonoBehaviour, IPoolable {
            public void Instantiated() {
                instantiateCalled = true;
            }

            public void Pooled() {
                pooledCalled = true;
            }
        }

        [Test]
        public void Pooling() {
            var prefab = new GameObject("Prefab");
            prefab.AddComponent<PoolableTest>();

            instantiateCalled = false;
            pooledCalled = false;

            var instance = Pool.Instantiate(prefab);
            Assert.AreNotSame(prefab, instance);

            var poolManaged = instance.GetComponent<PoolManaged>();
            Assert.IsNotNull(poolManaged);

            Assert.AreSame(prefab, poolManaged.Prefab);

            Assert.IsTrue(instantiateCalled);
            Assert.IsFalse(pooledCalled);
            instantiateCalled = false;
            pooledCalled = false;

            Pool.Destroy(instance);

            Assert.IsFalse(instantiateCalled);
            Assert.IsTrue(pooledCalled);
            instantiateCalled = false;
            pooledCalled = false;

            var instance2 = Pool.Instantiate(prefab);
            Assert.AreSame(instance, instance2);

            var poolManaged2 = instance2.GetComponent<PoolManaged>();
            Assert.IsNotNull(poolManaged2);

            Assert.AreSame(prefab, poolManaged2.Prefab);

            Assert.IsTrue(instantiateCalled);
            Assert.IsFalse(pooledCalled);
            instantiateCalled = false;
            pooledCalled = false;

            Pool.Destroy(instance2);
            pooledCalled = false;

            Pool.Cleanup();

            Assert.IsFalse(instance);
            Assert.IsFalse(instance2);
        }

        [Test]
        public void Instancing() {
            var prefab = new GameObject("Prefab");
            var instance1 = Pool.Instantiate(prefab);
            var instance2 = Pool.Instantiate(prefab);
            Assert.AreNotSame(instance1, instance2);
        }
    }
}
