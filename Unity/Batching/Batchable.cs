#define CUSTOM_BATCHING

using Hull.Unity.Pooling;
using UnityEngine;

namespace Hull.Unity.Batching {
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    [AddComponentMenu("Hull/Batchable")]
    public class Batchable : MonoBehaviour
#if CUSTOM_BATCHING
    , IPoolable
#endif 
    {
        private MeshFilter _meshFilter;
        internal int MeshVertexCount { get; private set; }
        public IVertexProcessor VertexProcessor;
        internal int MeshTrianglesCount { get; private set; }

        public Mesh Mesh {
            get { return _meshFilter.sharedMesh; }
        }
        
        public void MeshWasUpdated() {
#if CUSTOM_BATCHING
            var mesh = Mesh;
            if (mesh) {
                if (mesh.vertexCount != MeshVertexCount) {
                    RemoveFromCombinedMesh();
                    MeshVertexCount = mesh.vertexCount;
                    MeshTrianglesCount = Mesh.triangles.Length;
                    AddToCombinedMesh();
                }
                else if (!_addedToCombinedMesh) {
                    AddToCombinedMesh();
                }
                else {
                    _combinedMesh.UpdateBatchable(this);
                }
            }
#endif 
        }

#if CUSTOM_BATCHING
        private static CombinedMeshManager _combinedMeshManager;
        private CombinedMesh _combinedMesh;


        private MeshRenderer _meshRenderer;
        private bool _meshRendererWasEnabled;
        private bool _addedToCombinedMesh;


        private void Awake() {
            _meshFilter = GetComponent<MeshFilter>();
            Debug.Assert(_meshFilter);

            _meshRenderer = GetComponent<MeshRenderer>();
            Debug.Assert(_meshRenderer);
        }

        private void Start() {
            if (Mesh) {
                MeshVertexCount = Mesh.vertexCount;
                MeshTrianglesCount = Mesh.triangles.Length;
                transform.hasChanged = false;
                MeshWasUpdated();
            }
            else {
                transform.hasChanged = true;
            }
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
                _combinedMesh = _combinedMeshManager.GetCombinedMesh(gameObject.GetComponent<MeshRenderer>(), gameObject.GetComponent<MeshFilter>());
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
#endif
    }
}
