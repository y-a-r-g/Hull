using Hull.Unity.Pooling;
using UnityEngine;

namespace Hull.Unity.Batching {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Hull/Batchable")]
    public class Batchable : MonoBehaviour, IPoolable {
        private static CombinedMeshManager _combinedMeshManager;
        private CombinedMesh _combinedMesh;

        public IVertexProcessor VertexProcessor;

        private MeshFilter _meshFilter;
        private MeshRenderer _meshRenderer;
        private bool _meshRendererWasEnabled;
        private bool _addedToCombinedMesh;

        internal int MeshVertexCount { get; private set; }
        internal int MeshTrianglesCount { get; private set; }

        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
            Debug.Assert(_meshFilter);

            _meshRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(_meshRenderer);
        }

        private void Start() {
            MeshVertexCount = Mesh.vertexCount;
            MeshTrianglesCount = Mesh.triangles.Length;
            transform.hasChanged = false;
            MeshWasUpdated();
        }

        private void Update() {
            if (transform.hasChanged) {
                MeshWasUpdated();
                transform.hasChanged = false;
            }
        }

        public void Instantiated() {
            _meshRendererWasEnabled = _meshRenderer.enabled;
            _meshRenderer.enabled = false;
            MeshWasUpdated();
        }

        public void Pooled() {
            _meshRenderer.enabled = _meshRendererWasEnabled;
            RemoveFromCombinedMesh();
        }

        private void AddToCombinedMesh() {
            if (!_combinedMeshManager) {
                _combinedMeshManager = new GameObject("Hull.CombinedMeshManager").AddComponent<CombinedMeshManager>();
            }
            if (!_combinedMesh) {
                _combinedMesh = _combinedMeshManager.GetCombinedMesh(gameObject);
            }

            if (!_addedToCombinedMesh) {
                _combinedMesh.AddBatchable(this);
                _addedToCombinedMesh = true;
            }
        }

        private void RemoveFromCombinedMesh() {
            if (_addedToCombinedMesh) {
                _combinedMesh.RemoveBatchable(this);
                _addedToCombinedMesh = false;
            }
        }

        public void MeshWasUpdated() {
            var mesh = Mesh;
            if (mesh.vertexCount != MeshVertexCount) {
                RemoveFromCombinedMesh();
                MeshVertexCount = mesh.vertexCount;
                AddToCombinedMesh();
            }
            else if (!_addedToCombinedMesh) {
                AddToCombinedMesh();
            }
            else {
                _combinedMesh.UpdateBatchable(this);
            }
        }

        public Mesh Mesh {
            get { return _meshFilter.sharedMesh; }
        }
    }
}