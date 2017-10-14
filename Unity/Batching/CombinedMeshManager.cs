using System.Collections.Generic;
using UnityEngine;

namespace Hull.Unity.Batching {
    [AddComponentMenu("")]
    internal class CombinedMeshManager : MonoBehaviour {
        private readonly Dictionary<int, List<CombinedMesh>> _combinedMeshes = new Dictionary<int, List<CombinedMesh>>();

        public CombinedMesh GetCombinedMesh(MeshRenderer meshRenderer, MeshFilter meshFilter) {
            
            Debug.Assert(meshRenderer);
            var sharedMaterial = meshRenderer.sharedMaterial;
            var materialId = sharedMaterial.GetInstanceID();

            List<CombinedMesh> combinedMeshList;
            if (!_combinedMeshes.TryGetValue(materialId, out combinedMeshList)) {
                _combinedMeshes[materialId] = combinedMeshList = new List<CombinedMesh>();
            }

            var vertexCount = meshFilter.sharedMesh.vertexCount;
            CombinedMesh combinedMesh = null;
            
            foreach (var combinedMeshCandidate in combinedMeshList) {
                if (combinedMeshCandidate.FreeVerticesCapacity >= vertexCount) {
                    combinedMesh = combinedMeshCandidate;
                    break;
                }
            }

            if (!combinedMesh) {
                var go = new GameObject(string.Format("Hull.CombinedMesh.{0}", sharedMaterial.name));
                go.transform.SetParent(transform);
                var rendererCopy = go.AddComponent<MeshRenderer>();
                rendererCopy.material = sharedMaterial;
                rendererCopy.shadowCastingMode = meshRenderer.shadowCastingMode;
                rendererCopy.motionVectorGenerationMode = meshRenderer.motionVectorGenerationMode;
                rendererCopy.lightProbeUsage = meshRenderer.lightProbeUsage;
                rendererCopy.reflectionProbeUsage = meshRenderer.reflectionProbeUsage;
                go.AddComponent<MeshFilter>();
                combinedMeshList.Add(combinedMesh);
            }

            return combinedMesh;
        }
    }
}
