
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class FaceWireframe : MonoBehaviour
{
    [Header("Source References")]
    [SerializeField] private MeshFilter sourceMeshFilter;
    [SerializeField] private Material wireframeMaterial;

    [Header("Smoothing Settings")]
    [SerializeField, Range(0f, 1f)]
    private float smoothing = 0.5f;

    private MeshFilter wireMeshFilter;
    private MeshRenderer wireMeshRenderer;

    private Mesh wireMesh;

    private Vector3[] smoothedVertices;
    private int[] lineIndices;

    private void Awake()
    {
        wireMeshFilter = GetComponent<MeshFilter>();
        wireMeshRenderer = GetComponent<MeshRenderer>();

        wireMesh = new Mesh();
        wireMesh.name = "Face Wireframe Mesh";
        wireMesh.MarkDynamic();

        wireMeshFilter.sharedMesh = wireMesh;

        if (wireframeMaterial != null)
        {
            wireMeshRenderer.sharedMaterial = wireframeMaterial;
        }
    }

    private void LateUpdate()
    {
        if (sourceMeshFilter == null ||
            sourceMeshFilter.sharedMesh == null)
            return;

        Mesh source = sourceMeshFilter.sharedMesh;

        Vector3[] vertices = source.vertices;
        int[] triangles = source.triangles;

        if (vertices.Length == 0 || triangles.Length == 0)
            return;

        // Initialize smoothing arrays when mesh size changes
        if (smoothedVertices == null ||
            smoothedVertices.Length != vertices.Length)
        {
            smoothedVertices =
                (Vector3[])vertices.Clone();
            Debug.Log(
                $"MESH vertices={vertices.Length} " +
                $"uvs={source.uv.Length} " +
                $"indices={triangles.Length} " +
                $"normals={source.normals.Length}"
            );
        }
        else
        {
            // Smooth vertex movement to reduce jitter
            float smoothingFactor =
                1f - Mathf.Clamp01(smoothing);

            for (int i = 0; i < vertices.Length; i++)
            {
                smoothedVertices[i] = Vector3.Lerp(
                    smoothedVertices[i],
                    vertices[i],
                    smoothingFactor
                );
            }
        }

        // Generate line indices only when required
        if (lineIndices == null ||
            lineIndices.Length != triangles.Length * 2)
        {
            lineIndices = new int[triangles.Length * 2];

            int index = 0;

            for (int i = 0; i < triangles.Length; i += 3)
            {
                int a = triangles[i];
                int b = triangles[i + 1];
                int c = triangles[i + 2];

                // First triangle edge
                lineIndices[index++] = a;
                lineIndices[index++] = b;

                // Second triangle edge
                lineIndices[index++] = b;
                lineIndices[index++] = c;

                // Third triangle edge
                lineIndices[index++] = c;
                lineIndices[index++] = a;
            }
        }

        // Update wireframe mesh
        wireMesh.Clear();

        wireMesh.vertices = smoothedVertices;

        wireMesh.SetIndices(
            lineIndices,
            MeshTopology.Lines,
            0
        );

        wireMesh.RecalculateBounds();
    }

    private void OnDestroy()
    {
        if (wireMesh != null)
        {
            Destroy(wireMesh);
        }
    }
}
