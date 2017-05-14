using System.Collections.Generic;
using Hull.Extensions;
using UnityEngine;

namespace Hull.Unity.Batching {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    internal class CombinedMesh : MonoBehaviour {
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Vector4> _tangents = new List<Vector4>();
        private readonly List<Vector4> _uv0 = new List<Vector4>();
        private readonly List<Vector4> _uv1 = new List<Vector4>();
        private readonly List<Vector4> _uv2 = new List<Vector4>();
        private readonly List<Vector4> _uv3 = new List<Vector4>();
        private readonly List<Color32> _colors32 = new List<Color32>();
        private readonly List<int> _triangles = new List<int>();
        private bool _isModified;
        private MeshFilter _meshFilter;
        private readonly HashSet<Batchable> _batchables = new HashSet<Batchable>();

        public void AddBatchable(Batchable batchable) {
            _batchables.Add(batchable);
            _isModified = true;
        }

        public void RemoveBatchable(Batchable batchable) {
            _batchables.Remove(batchable);
            _isModified = true;
        }

        public void UpdateBatchable(Batchable batchable) {
            _isModified = true;
        }

        private void ProcessVertices(Batchable observer, int firstVertexIndex, int firstTriangleIndex) {
            if (observer.VertexProcessor != null) {
                observer.VertexProcessor.ProcessVertices(
                    observer.gameObject,
                    firstVertexIndex,
                    observer.MeshVertexCount,
                    _vertices,
                    _normals,
                    _tangents,
                    _uv0,
                    _uv1,
                    _uv2,
                    _uv3,
                    _colors32,
                    firstTriangleIndex,
                    observer.MeshTrianglesCount,
                    _triangles);
            }
        }

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void Update() {
            if (_isModified) {
                var mesh = _meshFilter.sharedMesh;
                if (!mesh) {
                    _meshFilter.mesh = mesh = new Mesh();
                }

                _vertices.Clear();
                _normals.Clear();
                _tangents.Clear();
                _uv0.Clear();
                _uv1.Clear();
                _uv2.Clear();
                _uv3.Clear();
                _colors32.Clear();
                _triangles.Clear();

                foreach (var batchable in _batchables) {
                    var batchableMesh = batchable.Mesh;
                    var emptyUVs = (IEnumerable<Vector4>)new Vector4[batchable.MeshVertexCount];
                    var uvs = new List<Vector4>();
                    var mx = batchable.transform.localToWorldMatrix;

                    var firstVertexIndex = _vertices.Count;
                    var firstTriangleIndex = _triangles.Count;

                    _vertices.AddRange(batchableMesh.vertices.Map(v => mx.MultiplyPoint(v)));
                    _normals.AddRange(batchableMesh.normals.Map(v => mx.MultiplyVector(v)));
                    _tangents.AddRange(batchableMesh.tangents);

                    batchableMesh.GetUVs(0, uvs);
                    _uv0.AddRange(uvs.Count == 0 ? emptyUVs : uvs);

                    uvs.Clear();
                    batchableMesh.GetUVs(1, uvs);
                    _uv1.AddRange(uvs.Count == 0 ? emptyUVs : uvs);

                    uvs.Clear();
                    batchableMesh.GetUVs(2, uvs);
                    _uv2.AddRange(uvs.Count == 0 ? emptyUVs : uvs);

                    uvs.Clear();
                    batchableMesh.GetUVs(3, uvs);
                    _uv3.AddRange(uvs.Count == 0 ? emptyUVs : uvs);

                    var colors32 = batchableMesh.colors32;
                    if (colors32.Length == 0) {
                        colors32 = new Color32[batchable.MeshVertexCount];
                    }
                    _colors32.AddRange(colors32);

                    _triangles.AddRange(batchableMesh.triangles.Map(i => i + firstVertexIndex));

                    ProcessVertices(batchable, firstVertexIndex, firstTriangleIndex);
                }

                mesh.Clear();
                if (_triangles.Count >= 0) {
                    mesh.SetVertices(_vertices);
                    mesh.SetNormals(_normals);
                    mesh.SetTangents(_tangents);
                    mesh.SetUVs(0, _uv0);
                    mesh.SetUVs(1, _uv1);
                    mesh.SetUVs(2, _uv2);
                    mesh.SetUVs(3, _uv3);
                    mesh.SetColors(_colors32);
                    mesh.SetTriangles(_triangles, 0);
                }

                _isModified = false;
            }
        }
    }
}
