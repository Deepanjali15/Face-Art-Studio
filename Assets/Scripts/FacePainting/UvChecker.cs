
using UnityEngine;

public class UvChecker : MonoBehaviour
{
    [SerializeField] private MeshRenderer target;
    [SerializeField] private int size = 512;
    [SerializeField] private int cells = 16;

    private void Start()
    {
        if (target == null)
        {
            Debug.LogError("UvChecker: Target MeshRenderer is not assigned!");
            return;
        }

        Texture2D tex = new Texture2D(
            size, size, TextureFormat.RGBA32, false
        );

        int cell = size / cells;

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool on = ((x / cell) + (y / cell)) % 2 == 0;

                tex.SetPixel(
                    x, y,
                    on ? Color.white : new Color(0.25f, 0.25f, 0.25f)
                );
            }
        }

        tex.Apply();

        target.material.mainTexture = tex;
    }
}
