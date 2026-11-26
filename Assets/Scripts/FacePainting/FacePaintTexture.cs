
using UnityEngine;

public class FacePaintTexture : MonoBehaviour
{
    [Header("Overlay Renderer")]
    [SerializeField] private Renderer faceRenderer;
    [SerializeField] private Material facePaintMaterial;

    [Header("Texture Settings")]
    [SerializeField] private int textureSize = 1024;

    private Texture2D paintTexture;
    private Material runtimeMaterial;
    private Material originalMaterial;
    private Color32[] pixels;

    public Texture2D PaintTexture => paintTexture;
    public int TextureSize => textureSize;

    // SAVE PAINTING DATA

    public Color32[] GetSavedPixels()
    {
        if (pixels == null)
            return null;

        return (Color32[])pixels.Clone();
    }

    // RESTORE PREVIOUS PAINTING

    public void RestorePixels(Color32[] savedPixels)
    {
        if (savedPixels == null || paintTexture == null)
            return;

        if (savedPixels.Length != pixels.Length)
            return;

        pixels = (Color32[])savedPixels.Clone();

        paintTexture.SetPixels32(pixels);
        paintTexture.Apply();

        Debug.Log("Previous face painting restored!");
    }

    private void Awake()
    {
        InitializeTexture();
    }

    // INITIALIZE PAINT TEXTURE

    private void InitializeTexture()
    {
        if (faceRenderer == null || facePaintMaterial == null)
        {
            Debug.LogError("Overlay Renderer or Paint Material missing!");
            return;
        }

        paintTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        paintTexture.name = "DynamicFacePaint";
        paintTexture.filterMode = FilterMode.Bilinear;
        paintTexture.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color32[textureSize * textureSize];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(255, 255, 255, 0);

        paintTexture.SetPixels32(pixels);
        paintTexture.Apply();

        originalMaterial = faceRenderer.sharedMaterial;
        runtimeMaterial = new Material(facePaintMaterial);

        if (runtimeMaterial.HasProperty("_BaseMap"))
            runtimeMaterial.SetTexture("_BaseMap", paintTexture);

        if (runtimeMaterial.HasProperty("_MainTex"))
            runtimeMaterial.SetTexture("_MainTex", paintTexture);

        faceRenderer.material = runtimeMaterial;

        Debug.Log("Face painting overlay initialized!");
    }

    // PAINT AT UV POSITION

    public void PaintAtUV(Vector2 uv, Color color, int radius)
    {
        if (paintTexture == null || pixels == null)
            return;

        radius = Mathf.Max(1, radius);

        int centerX = Mathf.RoundToInt(uv.x * (textureSize - 1));
        int centerY = Mathf.RoundToInt(uv.y * (textureSize - 1));

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radius * radius)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= textureSize ||
                    py < 0 || py >= textureSize)
                    continue;

                int index = py * textureSize + px;

                Color destination = pixels[index];
                float sourceAlpha = color.a;
                float destinationAlpha = destination.a;

                float outputAlpha = sourceAlpha +
                    destinationAlpha * (1f - sourceAlpha);

                if (outputAlpha <= 0f)
                    continue;

                Color output = new Color(
                    (color.r * sourceAlpha +
                     destination.r * destinationAlpha *
                     (1f - sourceAlpha)) / outputAlpha,

                    (color.g * sourceAlpha +
                     destination.g * destinationAlpha *
                     (1f - sourceAlpha)) / outputAlpha,

                    (color.b * sourceAlpha +
                     destination.b * destinationAlpha *
                     (1f - sourceAlpha)) / outputAlpha,

                    outputAlpha
                );

                pixels[index] = output;
            }
        }

        paintTexture.SetPixels32(pixels);
        paintTexture.Apply();
    }

    // ERASE AT UV POSITION

    public void EraseAtUV(Vector2 uv, int radius)
    {
        if (paintTexture == null || pixels == null)
            return;

        radius = Mathf.Max(1, radius);

        int centerX = Mathf.RoundToInt(uv.x * (textureSize - 1));
        int centerY = Mathf.RoundToInt(uv.y * (textureSize - 1));

        for (int y = -radius; y <= radius; y++)
        {
            for (int x = -radius; x <= radius; x++)
            {
                if (x * x + y * y > radius * radius)
                    continue;

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= textureSize ||
                    py < 0 || py >= textureSize)
                    continue;

                int index = py * textureSize + px;

                pixels[index] = new Color32(255, 255, 255, 0);
            }
        }

        paintTexture.SetPixels32(pixels);
        paintTexture.Apply();

        Debug.Log("Paint erased!");
    }

    // CLEAR ENTIRE PAINTING

    public void ClearPaint()
    {
        if (pixels == null || paintTexture == null)
            return;

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(255, 255, 255, 0);

        paintTexture.SetPixels32(pixels);
        paintTexture.Apply();

        Debug.Log("Face paint cleared!");
    }

    // CLEANUP

    private void OnDestroy()
    {
        if (faceRenderer != null && originalMaterial != null)
            faceRenderer.sharedMaterial = originalMaterial;

        if (runtimeMaterial != null)
            Destroy(runtimeMaterial);

        if (paintTexture != null)
            Destroy(paintTexture);
    }
}
