using System.Collections.Generic;
using UnityEngine;

namespace Hull.Unity.Rendering {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [AddComponentMenu("Hull/Planes")]
    public class PlanesMesh : MonoBehaviour {
        public struct Item {
            internal Vector3[] Vertices;
            internal Vector3[] Normals;
            internal Color32 Color;
            internal Vector2[] UVs;

            public Item(Vector3 pivot, Vector3 normal, Vector3 right, Rect uvs, Color32 color) {
                var up = Vector3.Cross(normal, right);
                Vertices = new[] {
                    pivot - right + up,
                    pivot + right + up,
                    pivot - right - up,
                    pivot + right - up
                };
                Normals = new[] {normal, normal, normal, normal};
                UVs = new[] {
                    new Vector2(uvs.xMin, uvs.yMax),
                    new Vector2(uvs.xMax, uvs.yMax),
                    new Vector2(uvs.xMin, uvs.yMin),
                    new Vector2(uvs.xMax, uvs.yMin)
                };
                Color = color;
            }

            public Item(Vector3[] vertices, Vector3[] normals, Vector2[] uvs, Color32 color) {
                Debug.Assert(vertices.Length == 4);
                Debug.Assert(normals.Length == 4);
                Debug.Assert(uvs.Length == 4);

                Vertices = vertices;
                Normals = normals;
                UVs = uvs;
                Color = color;
            }
        }

        private MeshFilter _meshFilter;
        private bool _isModified;
        private readonly List<Item> _planes = new List<Item>();
        private readonly List<Vector3> _vertices = new List<Vector3>();
        private readonly List<Vector3> _normals = new List<Vector3>();
        private readonly List<Vector2> _uvs = new List<Vector2>();
        private readonly List<Color32> _colors = new List<Color32>();
        private readonly List<int> _triangles = new List<int>();

        public void AddPlane(Item plane) {
            _planes.Add(plane);
            _isModified = true;
        }

        public void RemovePlane(Item plane) {
            _planes.Remove(plane);
            _isModified = true;
        }

        public void ClearPlanes() {
            _planes.Clear();
            _isModified = true;
        }

        public void AddPlanes(IEnumerable<Item> planes) {
            _planes.AddRange(planes);
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

                mesh.Clear();
                if (_planes.Count > 0) {
                    _vertices.Clear();
                    _normals.Clear();
                    _uvs.Clear();
                    _colors.Clear();
                    _triangles.Clear();

                    for (var i = 0; i < _planes.Count; i++) {
                        var plane = _planes[i];
                        var vertices = plane.Vertices;
                        _vertices.Add(vertices[0]);
                        _vertices.Add(vertices[1]);
                        _vertices.Add(vertices[2]);
                        _vertices.Add(vertices[3]);

                        var normals = plane.Normals;
                        _normals.Add(normals[0]);
                        _normals.Add(normals[1]);
                        _normals.Add(normals[2]);
                        _normals.Add(normals[3]);

                        var uvs = plane.UVs;
                        _uvs.Add(uvs[0]);
                        _uvs.Add(uvs[1]);
                        _uvs.Add(uvs[2]);
                        _uvs.Add(uvs[3]);

                        _colors.Add(plane.Color);
                        _colors.Add(plane.Color);
                        _colors.Add(plane.Color);
                        _colors.Add(plane.Color);

                        _triangles.Add(i * 4 + 0);
                        _triangles.Add(i * 4 + 2);
                        _triangles.Add(i * 4 + 1);

                        _triangles.Add(i * 4 + 1);
                        _triangles.Add(i * 4 + 2);
                        _triangles.Add(i * 4 + 3);
                    }

                    try {
                        mesh.SetVertices(_vertices);
                        mesh.SetNormals(_normals);
                        mesh.SetUVs(0, _uvs);
                        mesh.SetColors(_colors);
                        mesh.SetTriangles(_triangles, 0);
                    }
                    catch {
                        Debug.Break();
                    }
                }

                _isModified = false;
            }
        }
    }
}