#if MODULE_URP_ENABLED
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace UnityEngine.XR.ARFoundation
{
    public class ARCameraCompositionRendererFeature : ScriptableRendererFeature
    {
#if MODULE_URP_ENABLED
        [SerializeField]
        [Range(0, 1)]
        private float opacity = 0.9f;

        public float Opacity
        {
            set
            {
                opacity = value;
            }
            get
            {
                return opacity;
            }
        }

        private Material material;
        private ARCameraCompositionRenderPass renderPass = null;

        private readonly string shaderName = "AR Camera Composition/AR Camera Composition";

        public override void AddRenderPasses(ScriptableRenderer renderer,
                                        ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                renderer.EnqueuePass(renderPass);
            }
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer, in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                var currentCamera = renderingData.cameraData.camera;

                if (currentCamera != null)
                {
                    var cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                    if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                        && (cameraBackground.material != null))
                    {
                        var invertCulling = false;
                        if (cameraBackground.TryGetComponent<ARCameraManager>(out var arCameraManager))
                        {
                            invertCulling = arCameraManager.subsystem?.invertCulling ?? false;
                        }
                        renderPass.SetUp(cameraBackground, invertCulling, Opacity);
                    }
                }
            }

            renderPass.ConfigureInput(ScriptableRenderPassInput.Color);
            renderPass.SetRenderTarget(renderer.cameraColorTargetHandle);
        }

        public override void Create()
        {
            var shader = Shader.Find(shaderName);
            if (shader == null)
            {
                Debug.LogError($"Shader {shaderName} not found");
                return;
            }

            material = CoreUtils.CreateEngineMaterial(shader);
            renderPass = new ARCameraCompositionRenderPass(material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(material);
        }
#endif
    }
}
