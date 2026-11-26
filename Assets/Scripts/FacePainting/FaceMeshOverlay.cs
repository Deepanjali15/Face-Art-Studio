
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FaceMeshOverlay : MonoBehaviour
{
    private MeshFilter overlayMeshFilter;
    private MeshFilter sourceMeshFilter;

    private Mesh overlayMesh;

    [Header("Overlay Settings")]
    [SerializeField] private float surfaceOffset = 0.0005f;

    private void Awake()
    {
        overlayMeshFilter = GetComponent<MeshFilter>();

        if (transform.parent == null)
        {
            Debug.LogError("Face overlay has no parent!");
            return;
        }

        sourceMeshFilter =
            transform.parent.GetComponent<MeshFilter>();

        if (sourceMeshFilter == null)
        {
            Debug.LogError("Original face MeshFilter not found!");
            return;
        }

        overlayMesh = new Mesh();
        overlayMesh.name = "DynamicFacePaintOverlay";

        overlayMeshFilter.sharedMesh = overlayMesh;
    }

    private void LateUpdate()
    {
        if (sourceMeshFilter == null || overlayMesh == null)
            return;

        Mesh sourceMesh = sourceMeshFilter.sharedMesh;

        if (sourceMesh == null)
            return;

        Vector3[] vertices = sourceMesh.vertices;
        Vector3[] normals = sourceMesh.normals;

        Vector2[] uv = sourceMesh.uv;

        if (vertices.Length == 0 || uv.Length != vertices.Length)
            return;

        // Offset overlay slightly to avoid z-fighting
        if (normals.Length == vertices.Length)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i] += normals[i] * surfaceOffset;
            }
        }

        overlayMesh.Clear();

        overlayMesh.vertices = vertices;
        overlayMesh.uv = uv;

        if (normals.Length == vertices.Length)
            overlayMesh.normals = normals;
        else
            overlayMesh.RecalculateNormals();

        overlayMesh.subMeshCount = sourceMesh.subMeshCount;

        for (int i = 0; i < sourceMesh.subMeshCount; i++)
        {
            overlayMesh.SetTriangles(
                sourceMesh.GetTriangles(i),
                i
            );
        }

        overlayMesh.bounds = sourceMesh.bounds;
    }

    private void OnDestroy()
    {
        if (overlayMesh != null)
            Destroy(overlayMesh);
    }
}
