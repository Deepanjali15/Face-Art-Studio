
using UnityEngine;
using UnityEngine.XR.ARFoundation;

[RequireComponent(typeof(MeshCollider))]
public class FaceColliderSync : MonoBehaviour
{
    private MeshCollider meshCollider;
    private MeshFilter meshFilter;

    private void Awake()
    {
        meshCollider = GetComponent<MeshCollider>();
        meshFilter = GetComponent<MeshFilter>();
    }

    private void LateUpdate()
    {
        if (meshCollider == null || meshFilter == null)
            return;

        Mesh faceMesh = meshFilter.sharedMesh;

        if (faceMesh == null || faceMesh.vertexCount == 0)
            return;

        // Refresh the collider using the latest face mesh.
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = faceMesh;

        Physics.SyncTransforms();
    }
}
