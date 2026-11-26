
using System.Collections;
using UnityEngine;

public class SaveArtwork : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private Canvas facePaintCanvas;

    [Header("Gallery Settings")]
    [SerializeField] private string albumName = "Face Art Studio";

    public void SaveFaceArtwork()
    {
        StartCoroutine(CaptureAndSave());
    }

    private IEnumerator CaptureAndSave()
    {
        if (facePaintCanvas != null)
            facePaintCanvas.enabled = false;

        // Wait until the UI has disappeared from the screen.
        yield return new WaitForEndOfFrame();

        // Capture the complete AR camera view.
        Texture2D screenshot = ScreenCapture.CaptureScreenshotAsTexture();

        // Restore the painting controls.
        if (facePaintCanvas != null)
            facePaintCanvas.enabled = true;

        if (screenshot == null)
        {
            Debug.LogError("Could not capture artwork.");
            yield break;
        }

        byte[] imageBytes = screenshot.EncodeToPNG();

        NativeGallery.SaveImageToGallery(
            imageBytes,
            albumName,
            "FaceArtwork_" + System.DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".png",
            (success, path) =>
            {
                if (success)
                    Debug.Log("Artwork saved to gallery: " + path);
                else
                    Debug.LogError("Failed to save artwork.");
            }
        );

        Destroy(screenshot);
    }
}
