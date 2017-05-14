using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Hull.Unity.Pooling {
    public static class Pool {
        private static readonly Dictionary<GameObject, LinkedList<GameObject>> Pools =
            new Dictionary<GameObject, LinkedList<GameObject>>();

        private static Transform _poolTransform;

        public static GameObject Instantiate(GameObject prefab) {
            if (prefab == null) {
                throw new ArgumentNullException();
            }

            var pool = GetPool(prefab);
            GameObject instance;

            if (pool.First != null) {
                instance = pool.First.Value;
                pool.RemoveFirst();
                instance.transform.SetParent(null, false);
                instance.SetActive(true);
            }
            else {
                instance = Object.Instantiate(prefab);
                instance.AddComponent<PoolManaged>().Prefab = prefab;
            }
            CallRecursively(instance, true, false);
            return instance;
        }

        public static T Instantiate<T>(T component) where T : Component {
            if (component == null) {
                throw new ArgumentNullException();
            }
            return Instantiate(component.gameObject).GetComponent<T>();
        }

        public static void Destroy(GameObject gameObject) {
            if (!gameObject) {
                throw new ArgumentNullException();
            }

            gameObject.SetActive(false);

            var poolManaged = gameObject.GetComponent<PoolManaged>();

            CallRecursively(gameObject, false, true);
            if (poolManaged) {
                var pool = GetPool(poolManaged.Prefab);
                pool.AddLast(gameObject);

                if (!_poolTransform) {
                    _poolTransform = new GameObject("Hull.Pool").transform;
                }

                gameObject.transform.SetParent(_poolTransform, false);
            }
            else {
                SafeDestroy(gameObject);
            }
        }

        public static void Destroy<T>(T component) where T : Component {
            if (!component) {
                throw new ArgumentNullException();
            }

            Destroy(component.gameObject);
        }

        public static void Cleanup() {
            foreach (var keyValuePair in Pools) {
                foreach (var gameObject in keyValuePair.Value) {
                    SafeDestroy(gameObject);
                }
            }
            Pools.Clear();
        }

        private static LinkedList<GameObject> GetPool(GameObject prefab) {
            LinkedList<GameObject> pool;
            if (!Pools.TryGetValue(prefab, out pool)) {
                Pools[prefab] = pool = new LinkedList<GameObject>();
            }
            return pool;
        }

        private static void CallRecursively(GameObject gameObject, bool init, bool release) {
            var poolables = gameObject.GetComponents<IPoolable>();
            foreach (var poolable in poolables) {
                if (init) {
                    poolable.Instantiated();
                }
                if (release) {
                    poolable.Pooled();
                }
            }

            var eChildren = gameObject.transform.GetEnumerator();
            while (eChildren.MoveNext()) {
                CallRecursively(((Component)eChildren.Current).gameObject, init, release);
            }
        }

        private static void SafeDestroy(GameObject gameObject) {
            if (!Application.isPlaying) {
                Object.DestroyImmediate(gameObject);
            }
            else {
                Object.Destroy(gameObject);
            }
        }
    }
}
