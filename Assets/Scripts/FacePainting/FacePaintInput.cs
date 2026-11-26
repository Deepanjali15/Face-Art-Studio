
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;

public class FacePaintInput : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera arCamera;
    [SerializeField] private ARFaceManager faceManager;
    [SerializeField] private FacePaintBrush paintBrush;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 2f;

    private Vector2 previousUV;
    private bool hasPreviousUV;

    private void Awake()
    {
        if (arCamera == null)
            arCamera = Camera.main;

        if (faceManager == null)
            faceManager = FindAnyObjectByType<ARFaceManager>();

        if (paintBrush == null)
            paintBrush = FindAnyObjectByType<FacePaintBrush>();
    }

    private void Update()
    {
        if (Touchscreen.current == null)
        {
            hasPreviousUV = false;
            return;
        }

        var touch = Touchscreen.current.primaryTouch;
        var phase = touch.phase.ReadValue();

        if (phase == UnityEngine.InputSystem.TouchPhase.Began)
        {
            Debug.Log("TOUCH DETECTED");

            hasPreviousUV = false;
            PaintAtScreenPosition(touch.position.ReadValue());
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Moved ||
                 phase == UnityEngine.InputSystem.TouchPhase.Stationary)
        {
            PaintAtScreenPosition(touch.position.ReadValue());
        }
        else if (phase == UnityEngine.InputSystem.TouchPhase.Ended ||
                 phase == UnityEngine.InputSystem.TouchPhase.Canceled)
        {
            hasPreviousUV = false;
        }
    }

    private void PaintAtScreenPosition(Vector2 screenPosition)
    {
        if (arCamera == null || paintBrush == null ||
            faceManager == null)
            return;

        Ray ray = arCamera.ScreenPointToRay(screenPosition);

        if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance))
        {
            Debug.Log("RAYCAST HIT: " + hit.collider.name);

            ARFace face = hit.collider.GetComponent<ARFace>();

            if (face == null)
                face = hit.collider.GetComponentInParent<ARFace>();

            if (face == null)
                return;

            Vector2 currentUV = hit.textureCoord;

            if (!hasPreviousUV)
            {
                paintBrush.Paint(currentUV);
                previousUV = currentUV;
                hasPreviousUV = true;
                return;
            }

            float distance = Vector2.Distance(previousUV, currentUV);
            int steps = Mathf.Clamp(
                Mathf.CeilToInt(distance * 100f), 1, 30);

            for (int i = 1; i <= steps; i++)
            {
                Vector2 uv = Vector2.Lerp(
                    previousUV, currentUV, (float)i / steps);

                paintBrush.Paint(uv);
            }

            previousUV = currentUV;
        }
        else
        {
            Debug.Log("RAYCAST MISSED THE FACE");
            hasPreviousUV = false;
        }
    }
}
