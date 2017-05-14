using System.Collections.Generic;
using UnityEngine;

namespace Hull.Unity.Batching {
    [AddComponentMenu("")]
    internal class CombinedMeshManager : MonoBehaviour {
        private readonly Dictionary<int, CombinedMesh> _combinedMeshes = new Dictionary<int, CombinedMesh>();

        public CombinedMesh GetCombinedMesh(GameObject gameObjectWithMesh) {
            var meshRenderer = gameObjectWithMesh.GetComponent<MeshRenderer>();
            Debug.Assert(meshRenderer);
            var sharedMaterial = meshRenderer.sharedMaterial;
            var materialId = sharedMaterial.GetInstanceID();

            CombinedMesh combinedMesh;
            if (!_combinedMeshes.TryGetValue(materialId, out combinedMesh)) {
                var go = new GameObject(string.Format("Hull.CombinedMesh.{0}", sharedMaterial.name));
                go.transform.SetParent(transform);
                var rendererCopy = go.AddComponent<MeshRenderer>();
                rendererCopy.material = sharedMaterial;
                rendererCopy.shadowCastingMode = meshRenderer.shadowCastingMode;
                rendererCopy.motionVectorGenerationMode = meshRenderer.motionVectorGenerationMode;
                rendererCopy.lightProbeUsage = meshRenderer.lightProbeUsage;
                rendererCopy.reflectionProbeUsage = meshRenderer.reflectionProbeUsage;
                go.AddComponent<MeshFilter>();
                _combinedMeshes[materialId] = combinedMesh = go.AddComponent<CombinedMesh>();
            }

            return combinedMesh;
        }
    }
}
