# Face Art Studio (AR FaceCanvas)

A real-time **augmented reality face-painting app for Android**. Open the front camera, pick a colour, and paint directly on your own face. The paint is attached to the tracked 3D face mesh, so it follows your head as you move.

Built with **Unity 6**, **AR Foundation** and **Google ARCore (Augmented Faces)**.

> Course project for *21CSE353T – Augmented, Virtual and Mixed Reality*, B.Tech Computer Science and Engineering, SRM Institute of Science and Technology, Kattankulathur.

## Download

Grab the latest APK from the [Releases page](https://github.com/Deepanjali15/Face-Art-Studio/releases/latest) and install it on an Android phone.

**Requirement:** the phone must support **ARCore Augmented Faces** (front-camera face tracking). Check the [ARCore supported devices list](https://developers.google.com/ar/devices). Not every ARCore phone supports it.

## Screenshots

<!-- Add screenshots to a docs/ folder and link them here, e.g.
![Painting on a face](docs/painting.png)
![Colour palette and controls](docs/ui.png)
-->

## Features

- Real-time face detection and tracking through the front camera (one face)
- Freehand painting on the face by touch, mapped through the face mesh UV coordinates
- Colour palette and Small / Medium / Large brush presets
- Eraser, Paint mode and Clear All
- Paint stays aligned with the face while you move your head
- Paint is kept and restored when tracking is lost and the face comes back into view
- Save Artwork: captures the camera view with the UI hidden and saves a PNG to the phone's gallery (album "Face Art Studio")
- Developer tools: face wireframe overlay and a debug HUD (tracking state, FPS, vertex count)

## How it works

```
Front camera -> ARCore face tracking -> AR Foundation (ARFaceManager)
   -> CustomFacePrefab (face mesh + Mesh Collider + paint overlay)
Touch -> ray from AR camera -> raycast hits face collider -> hit.textureCoord (UV)
   -> FacePaintBrush -> FacePaintTexture (updates RGBA32 paint texture)
   -> paint overlay renders the texture on top of the live camera image
```

1. **Tracking:** ARCore supplies the face mesh; AR Foundation exposes it as an `ARFace`.
2. **Overlay:** a separate transparent mesh copies the tracked face mesh, so unpainted areas show the real camera image.
3. **Touch to UV:** a camera ray hits the face's Mesh Collider, and the hit's UV coordinate is the position on the paint texture.
4. **Painting:** the brush updates pixels in a runtime transparent texture. Strokes are interpolated between UV points so fast movement draws a continuous line.
5. **Recovery:** the paint pixels are stored when the face is lost and restored when the face is tracked again.

## Scripts

All scripts are in `Assets/Scripts/FacePainting/`.

| Script | Purpose |
|---|---|
| `FaceTrackingManager` | Listens to `ARFaceManager`, keeps the current face, fires face found / lost events |
| `FaceSmoothing` | Smooths the position and rotation of an anchor that follows the face |
| `FaceMeshOverlay` | Copies the tracked face mesh onto the paint overlay, with a small surface offset |
| `FaceColliderSync` | Keeps the Mesh Collider in sync with the updating face mesh |
| `FacePaintInput` | Reads touch input, raycasts onto the face, forwards UV coordinates |
| `FacePaintBrush` | Colour, brush size, paint/erase mode, clear, save and restore of paint pixels |
| `FacePaintTexture` | Owns the runtime paint texture and material; paint, erase, clear |
| `FacePaintUI` | Hooks UI buttons and the brush-size slider to the brush |
| `SaveArtwork` | Hides the UI, captures a screenshot and saves it to the gallery |
| `FaceWireframe` | Debug wireframe of the tracked face mesh |
| `FaceDebugHUD` | Debug text: tracking state, FPS, vertex count |
| `UvChecker`, `UvTemplateExporter` | Helpers for inspecting the face mesh UV layout and exporting a UV template PNG |

## Tech stack

- Unity **6000.6.8f1** (Unity 6)
- C#
- AR Foundation + Google ARCore XR Plugin
- Universal Render Pipeline (URP)
- Unity Input System (new)
- TextMeshPro
- [NativeGallery](https://github.com/yasirkula/UnityNativeGallery) for saving images to the Android gallery

## Run it from source

1. Install **Unity 6000.6.8f1** through Unity Hub, with **Android Build Support** (SDK, NDK, OpenJDK).
2. Clone the repo. It uses Git LFS for large binary assets, so install [Git LFS](https://git-lfs.com/) first.
   ```bash
   git lfs install
   git clone https://github.com/Deepanjali15/Face-Art-Studio.git
   ```
3. Open the project folder in Unity Hub and let it import.
4. Open the main scene (the `FaceStudio` scene in `Assets/Scenes`).
5. Go to **File > Build Settings**, switch the platform to **Android**.
6. In **Project Settings > XR Plug-in Management**, make sure **ARCore** is enabled for Android.
7. Connect an ARCore face-tracking capable phone with USB debugging on, then **Build And Run**.

The app does not run in the Unity Editor. It needs a real device with front-camera face tracking.

## Testing

Tested on a **Samsung Galaxy S23 Ultra**. Observed in one diagnostic session: about 26 FPS and 468 face-mesh vertices. That is a single reading, not a benchmark, and results will vary by device and lighting.

## Known limitations

- Needs a phone that supports ARCore Augmented Faces
- Tested on one device only
- Tracking quality depends on lighting and face position; extreme head turns can interrupt tracking
- No in-app gallery, undo/redo, symmetry, layers or paint templates yet
- Saved artwork goes to the phone gallery; there is no in-app viewer

## Future work

Gallery and sharing, brush opacity and more brush styles, custom colour picker, undo/redo, symmetry drawing, performance profiling on more devices.

## Team

- Deepanjali Singh
- KVSL Geetika Reddy

## Credits and license

- [NativeGallery](https://github.com/yasirkula/UnityNativeGallery) by Yasin Kula (used for saving to the gallery)
- Unity, AR Foundation and Google ARCore documentation and samples

See the [LICENSE](LICENSE) file for this project's license.
