using UnityEngine;

namespace Hull.Extensions {
    public static class BoundsExtensions {
        public static Vector3 Clamp(this Bounds bounds, Vector3 position) {
            return new Vector3(
                    Mathf.Clamp(position.x, bounds.min.x, bounds.max.x),
                    Mathf.Clamp(position.y, bounds.min.y, bounds.max.y),
                    Mathf.Clamp(position.z, bounds.min.z, bounds.max.z)
                );
        }
    }
}
