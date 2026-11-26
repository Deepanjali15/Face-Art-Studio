
using System.IO;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class UvTemplateExporter : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private FaceTrackingManager trackingManager;

    [Header("Export Settings")]
    [SerializeField] private int textureSize = 1024;

    private bool exported = false;

    private void Update()
    {
        if (exported || trackingManager == null)
            return;

        if (trackingManager.IsFaceTracked &&
            trackingManager.CurrentFace != null)
        {
            ExportUVTemplate();
        }
    }

    private void ExportUVTemplate()
    {
        ARFace face = trackingManager.CurrentFace;

        MeshFilter meshFilter =
            face.GetComponent<MeshFilter>();

        if (meshFilter == null)
            meshFilter = face.GetComponentInChildren<MeshFilter>();

        if (meshFilter == null ||
            meshFilter.sharedMesh == null)
        {
            Debug.LogWarning("Face mesh not available yet.");
            return;
        }

        Mesh mesh = meshFilter.sharedMesh;

        Vector2[] uvs = mesh.uv;
        int[] triangles = mesh.triangles;

        if (uvs == null || uvs.Length == 0 ||
            triangles == null || triangles.Length == 0)
        {
            Debug.LogError("Face mesh UV data is missing.");
            return;
        }

        Texture2D texture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        Color32[] pixels =
            new Color32[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(255, 255, 255, 255);

        texture.SetPixels32(pixels);

        for (int i = 0; i < triangles.Length; i += 3)
        {
            int a = triangles[i];
            int b = triangles[i + 1];
            int c = triangles[i + 2];

            DrawLine(texture, uvs[a], uvs[b]);
            DrawLine(texture, uvs[b], uvs[c]);
            DrawLine(texture, uvs[c], uvs[a]);
        }

        texture.Apply();

        string folder = Path.Combine(
            Application.persistentDataPath,
            "FaceArtStudio"
        );

        Directory.CreateDirectory(folder);

        string path = Path.Combine(
            folder,
            "FaceUVTemplate.png"
        );

        File.WriteAllBytes(path, texture.EncodeToPNG());

        Debug.Log("UV TEMPLATE EXPORTED: " + path);

        Destroy(texture);

        exported = true;
    }

    private void DrawLine(
        Texture2D texture,
        Vector2 start,
        Vector2 end)
    {
        int x0 = Mathf.RoundToInt(
            start.x * (textureSize - 1));

        int y0 = Mathf.RoundToInt(
            start.y * (textureSize - 1));

        int x1 = Mathf.RoundToInt(
            end.x * (textureSize - 1));

        int y1 = Mathf.RoundToInt(
            end.y * (textureSize - 1));

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);

        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;

        int error = dx - dy;

        while (true)
        {
            if (x0 >= 0 && x0 < textureSize &&
                y0 >= 0 && y0 < textureSize)
            {
                texture.SetPixel(x0, y0, Color.black);
            }

            if (x0 == x1 && y0 == y1)
                break;

            int e2 = 2 * error;

            if (e2 > -dy)
            {
                error -= dy;
                x0 += sx;
            }

            if (e2 < dx)
            {
                error += dx;
                y0 += sy;
            }
        }
    }
}
