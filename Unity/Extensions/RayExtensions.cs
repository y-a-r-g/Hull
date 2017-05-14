using UnityEngine;

namespace Hull.Unity.Extensions {
    public static class RayExtensions {
        public static bool IntersectsTriangle(
            this Ray ray,
            Vector3 tri0,
            Vector3 tri1,
            Vector3 tri2,
            out Vector3 intersection) {
            intersection = Vector3.zero;

            // get triangle edge vectors and plane normal
            var u = tri1 - tri0;
            var v = tri2 - tri0;
            var n = Vector3.Cross(u, v);
            if (n.magnitude < Mathf.Epsilon) {
                return false; //-1;                  // do not deal with this case
            }

            var dir = ray.direction;
            var w0 = ray.origin - tri0;
            var a = -Vector3.Dot(n, w0);
            var b = Vector3.Dot(n, dir);
            if (Mathf.Abs(b) < Mathf.Epsilon) { // ray is  parallel to triangle plane
                if (Mathf.Abs(a) < Mathf.Epsilon) { // ray lies in triangle plane
                    return false; //2;
                }
                return false; //0;              // ray disjoint from plane
            }

            // get intersect point of ray with triangle plane
            var r = a / b;
            if (r < 0) { // ray goes away from triangle
                return false; //0;                   // => no intersect
            }

            // for a segment, also test if (r > 1.0) => no intersect
            intersection = ray.origin + r * dir; // intersect point of ray and plane

            // is intersection inside T?
            var uu = Vector3.Dot(u, u);
            var uv = Vector3.Dot(u, v);
            var vv = Vector3.Dot(v, v);
            var w = intersection - tri0;
            var wu = Vector3.Dot(w, u);
            var wv = Vector3.Dot(w, v);
            var d = uv * uv - uu * vv;

            // get and test parametric coords
            var s = (uv * wv - vv * wu) / d;
            if (s < 0.0 || s > 1.0) { // intersection is outside T
                return false; //0;
            }
            var t = (uv * wu - uu * wv) / d;
            if (t < 0.0 || (s + t) > 1.0) { // intersection is outside T
                return false; //0;
            }

            return true; //1;                       // intersection is in T
        }

        public static float DistanceToPoint(this Ray ray, Vector3 point) {
            return Vector3.Cross(ray.direction, point - ray.origin).magnitude;
        }
    }
}
