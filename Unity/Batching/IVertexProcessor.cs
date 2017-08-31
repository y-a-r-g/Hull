using System.Collections.Generic;
using UnityEngine;

namespace Hull.Unity.Batching {
    public interface IVertexProcessor {
        void ProcessVertices(
            GameObject observableObject,
            int vertexIndex,
            int vertexCount,
            List<Vector3> vertices,
            List<Vector3> normals,
            List<Vector4> tangents,
            List<Vector4> uv0,
            List<Vector4> uv1,
            List<Vector4> uv2,
            List<Vector4> uv3,
            List<Color32> colors32,
            int triangleIndex,
            int triangleCount,
            List<int> triangles);
    }
}