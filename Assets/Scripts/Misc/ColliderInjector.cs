using BasicTools.ButtonInspector;
using UnityEngine;

public class ColliderInjector : MonoBehaviour
{
    [Button("Generate Mesh Colliders", "GenerateMeshColliders")]
    public bool generateMeshColliders_Btn;
    
    [Button("Remove Mesh Colliders", "RemoveMeshColliders")]
    public bool removeMeshColliders_Btn;

    public void GenerateMeshColliders()
    {
        foreach (var meshRenderer in GetComponentsInChildren<MeshRenderer>())
        {
            var meshCollider = meshRenderer.GetComponent<MeshCollider>();
            if (meshCollider == null)
            {
                meshRenderer.gameObject.AddComponent<MeshCollider>();
            }
        }
    }
    
    public void RemoveMeshColliders()
    {
        foreach (var meshCollider in GetComponentsInChildren<MeshCollider>())
        {
            DestroyImmediate(meshCollider);
        }
    }
}