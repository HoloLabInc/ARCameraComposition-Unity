# ARCameraComposition

AR Camera Composition is a module for AR Foundation that enables compositing of AR content and camera image with adjustable transparency.

<img width="480" alt="AR Camera Composition Demo" src="https://user-images.githubusercontent.com/4415085/220044421-d22168c3-1666-4d2e-9bc9-6f6162484193.gif">

## Requirements

- Universal Render Pipeline (URP) 14 or higher
- AR Foundation

> [!NOTE]
> If you are using URP 13 or lower, please use the [ARCameraComposition v0.2.0](https://github.com/HoloLabInc/ARCameraComposition-Unity/blob/v0.2.0/README.md).

## Install

Open `Packages\manifest.json` and add this line in "dependencies".

```
"jp.co.hololab.arcameracomposition": "https://github.com/HoloLabInc/ARCameraComposition-Unity.git?path=packages/jp.co.hololab.arcameracomposition#v1.0.0",
```

## Usage

### Setup render pipeline settings

Select **Edit** > **Project Settings** > **Graphics** and assign the ARCameraComposition_UniversalRenderPipelineAsset, which is included in the AR Camera Compotsition package.

<img width="640" alt="Scriptable Render Pipeline Settings" src="https://user-images.githubusercontent.com/4415085/219991309-930e7b1b-45ff-4527-accc-72fb6f311912.png">

<br>

If you use your custom renderer pipeline asset, please add "AR Background Renderer Feature" and "AR Camera Composition Renderer Feature" to your renderer.

<img width="480" alt="Custom renderer" src="https://github.com/HoloLabInc/ARCameraComposition-Unity/assets/4415085/21b82a24-46aa-415f-b48b-69c0c1c0a131">

### Try out the sample scene

Open the Package Manager window.  
Select "AR Camera Composition" and press the "Import" button.

<img width="480" alt="Import sample scene" src="https://github.com/HoloLabInc/ARCameraComposition-Unity/assets/4415085/ebee5eec-a9d3-4350-843b-b737c52fb610">

## License

MIT
