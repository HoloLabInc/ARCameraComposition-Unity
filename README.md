# ARCameraComposition
AR Camera Composition is a module for AR Foundation that enables compositing of AR content and camera image with adjustable transparency.

<img width="480" alt="AR Camera Composition Demo" src="https://user-images.githubusercontent.com/4415085/220044421-d22168c3-1666-4d2e-9bc9-6f6162484193.gif">

## Requirements
- Universal Render Pipeline (URP)
- AR Foundation

## Install
Open `Packages\manifest.json` and add this line in "dependencies".

```
"jp.co.hololab.arcameracomposition": "https://github.com/HoloLabInc/ARCameraComposition-Unity.git?path=packages/jp.co.hololab.arcameracomposition",
```

## Usage
### Setup render pipeline settings
Select **Edit** > **Project Settings** > **Graphics** and assign the ARCameraComposition_UniversalRenderPipelineAsset, which is included in the AR Camera Compotsition package.

<img width="640" alt="Scriptable Render Pipeline Settings" src="https://user-images.githubusercontent.com/4415085/219991309-930e7b1b-45ff-4527-accc-72fb6f311912.png">

<br>

If you use your custom renderer pipeline asset, please add "AR Camera Composition Renderer Feature" to your renderer.

<img width="480" alt="Custom renderer" src="https://user-images.githubusercontent.com/4415085/220015858-2349b77a-10dd-453f-bbb6-7f570594c69a.png">


### Import sample scene
Open the Package Manager window.  
Select "AR Camera Composition" and press the "Import" button.

<img width="480" alt="Import sample scene" src="https://user-images.githubusercontent.com/4415085/220215799-f1253dc5-efe7-4023-ae03-da2e8ae8720f.png">

## License
Unity Companion License
