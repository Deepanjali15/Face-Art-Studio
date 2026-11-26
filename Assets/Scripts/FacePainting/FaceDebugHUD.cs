
using TMPro;
using UnityEngine;

public class FaceDebugHUD : MonoBehaviour
{
    [SerializeField] private FaceTrackingManager tracking;
    [SerializeField] private TMP_Text label;

    private float fps;

    private void Update()
    {
        if (tracking == null || label == null)
            return;

        fps = Mathf.Lerp(
            fps,
            1f / Mathf.Max(Time.unscaledDeltaTime, 0.0001f),
            0.1f
        );

        string state = tracking.IsFaceTracked
            ? "TRACKING"
            : "NO FACE";

        int verts = tracking.CurrentFace != null
            ? tracking.CurrentFace.vertices.Length
            : 0;

        label.text = $"{state}   FPS {fps:0}   verts {verts}";
    }
}
