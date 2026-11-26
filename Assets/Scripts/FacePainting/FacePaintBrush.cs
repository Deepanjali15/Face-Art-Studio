
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FacePaintBrush : MonoBehaviour
{
    [Header("AR Face Tracking")]
    [SerializeField] private ARFaceManager faceManager;

    [Header("Brush Settings")]
    [SerializeField] private Color brushColor = Color.red;
    [SerializeField] private int brushRadius = 20;

    private FacePaintTexture paintTexture;
    private ARFace activeFace;

    private Color32[] savedPixels;
    private bool isErasing = false;

    private void OnEnable()
    {
        if (faceManager == null)
            faceManager = GetComponent<ARFaceManager>();

        if (faceManager == null)
            faceManager = FindAnyObjectByType<ARFaceManager>();

        if (faceManager != null)
        {
            faceManager.trackablesChanged.AddListener(OnFacesChanged);
            Debug.Log("FacePaintBrush connected to AR Face Manager.");
        }
        else
        {
            Debug.LogError("AR Face Manager is not assigned!");
        }
    }

    private void OnDisable()
    {
        if (faceManager != null)
            faceManager.trackablesChanged.RemoveListener(OnFacesChanged);
    }

    private void OnFacesChanged(
        ARTrackablesChangedEventArgs<ARFace> args)
    {
        // Handle newly detected faces.
        foreach (ARFace face in args.added)
        {
            if (face.trackingState == TrackingState.Tracking)
                FindPaintTexture(face);
        }

        // Handle faces whose tracking state has changed.
        foreach (ARFace face in args.updated)
        {
            if (face.trackingState == TrackingState.Tracking)
            {
                FindPaintTexture(face);
            }
            else
            {
                SaveAndReleaseFace(face);
            }
        }

        // Handle completely removed faces.
        foreach (var item in args.removed)
        {
            SaveAndReleaseFace(item.Value);
        }
    }

    private void FindPaintTexture(ARFace face)
    {
        if (face == null)
            return;

        if (activeFace == face && paintTexture != null)
            return;

        paintTexture = face.GetComponentInChildren<FacePaintTexture>();

        if (paintTexture == null)
        {
            Debug.LogWarning("FacePaintTexture not found on tracked face.");
            return;
        }

        activeFace = face;

        Debug.Log("Face painting target detected!");

        if (savedPixels != null)
        {
            paintTexture.RestorePixels(savedPixels);
            Debug.Log("Previous painting restored!");
        }
    }

    private void SaveAndReleaseFace(ARFace face)
    {
        if (face == null || activeFace != face)
            return;

        if (paintTexture != null)
        {
            savedPixels = paintTexture.GetSavedPixels();

            Debug.Log("Face tracking lost. Painting saved!");
        }

        paintTexture = null;
        activeFace = null;
    }

    // ---------------- COLOR SETTINGS ----------------

    public void SetColor(Color newColor)
    {
        brushColor = newColor;
        isErasing = false;
    }

    public void SetRed()
    {
        SetColor(Color.red);
    }

    public void SetBlue()
    {
        SetColor(Color.blue);
    }

    public void SetGreen()
    {
        SetColor(Color.green);
    }

    public void SetYellow()
    {
        SetColor(Color.yellow);
    }

    public void SetPink()
    {
        SetColor(new Color(1f, 0.4f, 0.7f));
    }

    public void SetPurple()
    {
        SetColor(new Color(0.6f, 0.2f, 0.9f));
    }

    public void SetWhite()
    {
        SetColor(Color.white);
    }

    // ---------------- BRUSH SIZE ----------------

    public void SetBrushSize(int newSize)
    {
        brushRadius = Mathf.Max(1, newSize);
    }

    public void SetSmallBrush()
    {
        SetBrushSize(8);
    }

    public void SetMediumBrush()
    {
        SetBrushSize(20);
    }

    public void SetLargeBrush()
    {
        SetBrushSize(40);
    }

    // ---------------- PAINT / ERASER ----------------

    public void SetEraser()
    {
        isErasing = true;
        Debug.Log("Eraser mode enabled!");
    }

    public void SetPaintBrush()
    {
        isErasing = false;
        Debug.Log("Paint mode enabled!");
    }

    public void Paint(Vector2 uv)
    {
        if (paintTexture == null)
        {
            Debug.LogWarning("No tracked face painting target found!");
            return;
        }

        if (isErasing)
        {
            paintTexture.EraseAtUV(uv, brushRadius);
        }
        else
        {
            paintTexture.PaintAtUV(uv, brushColor, brushRadius);
        }
    }

    // ---------------- CLEAR ALL ----------------

    public void Clear()
    {
        if (paintTexture != null)
            paintTexture.ClearPaint();

        savedPixels = null;

        Debug.Log("All face painting cleared!");
    }
}
