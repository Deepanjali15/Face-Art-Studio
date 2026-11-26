
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.ARFoundation;

public class FaceTrackingManager : MonoBehaviour
{
    [Header("AR Face Manager")]
    [SerializeField] private ARFaceManager faceManager;

    [Header("Smoothing")]
    [SerializeField] private FaceSmoothing smoothedAnchor;

    [Header("Face Tracking Events")]
    public UnityEvent onFaceFound;
    public UnityEvent onFaceLost;

    public ARFace CurrentFace { get; private set; }

    public bool IsFaceTracked =>
        CurrentFace != null &&
        CurrentFace.trackingState ==
        UnityEngine.XR.ARSubsystems.TrackingState.Tracking;

    private void OnEnable()
    {
        if (faceManager == null)
            faceManager = FindAnyObjectByType<ARFaceManager>();

        if (faceManager != null)
            faceManager.trackablesChanged.AddListener(OnFacesChanged);
        else
            Debug.LogError("AR Face Manager not found!");
    }

    private void OnDisable()
    {
        if (faceManager != null)
            faceManager.trackablesChanged.RemoveListener(OnFacesChanged);
    }

    private void OnFacesChanged(
        ARTrackablesChangedEventArgs<ARFace> args)
    {
        foreach (ARFace face in args.added)
        {
            SetCurrentFace(face);
        }

        foreach (ARFace face in args.updated)
        {
            if (CurrentFace == null)
                SetCurrentFace(face);
        }

        foreach (var item in args.removed)
        {
            if (CurrentFace == item.Value)
            {
                CurrentFace = null;

                if (smoothedAnchor != null)
                    smoothedAnchor.target = null;

                Debug.Log("Face Lost!");
                onFaceLost?.Invoke();
            }
        }
    }

    private void SetCurrentFace(ARFace face)
    {
        if (face == null || CurrentFace == face)
            return;

        CurrentFace = face;

        if (smoothedAnchor != null)
            smoothedAnchor.target = face.transform;

        Debug.Log("Face Found!");
        onFaceFound?.Invoke();
    }
}
