using Hull.Unity.Pooling;
using UnityEngine;

namespace Hull.Unity.Extensions {
    public static class TransformExtensions {
        public static void RemoveAllChildren(this Transform transform) {
            while (transform.childCount > 0) {
                Pool.Destroy(transform.GetChild(0));
            }
        }
    }
}
