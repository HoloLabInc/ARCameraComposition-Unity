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
        public Shader m_Shader;

        [SerializeField]
        [Range(0, 1)]
        private float opacity = 0.9f;

        public float Opacity { set; get; }

        Material m_Material;

        ARCameraCompositionRenderPass m_RenderPass = null;

        public override void AddRenderPasses(ScriptableRenderer renderer,
                                        ref RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
                renderer.EnqueuePass(m_RenderPass);
        }

        public override void SetupRenderPasses(ScriptableRenderer renderer,
                                            in RenderingData renderingData)
        {
            if (renderingData.cameraData.cameraType == CameraType.Game)
            {
                var currentCamera = renderingData.cameraData.camera;

                if (currentCamera != null)
                {
                    ARCameraBackground cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();
                    if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                        && (cameraBackground.material != null))
                    // && TrySelectRenderPassForBackgroundRenderMode(cameraBackground.currentRenderingMode, out var renderPass))
                    {
                        // var invertCulling = cameraBackground.GetComponent<ARCameraManager>()?.subsystem?.invertCulling ?? false;
                        // renderPass.Setup(cameraBackground, invertCulling);
                        // renderer.EnqueuePass(renderPass);
                        m_RenderPass.Setup(cameraBackground);
                    }
                }


                // Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
                // ensures that the opaque texture is available to the Render Pass.
                m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
                m_RenderPass.SetTarget(renderer.cameraColorTargetHandle, Opacity);
            }
        }

        public override void Create()
        {
            m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
            m_RenderPass = new ARCameraCompositionRenderPass(m_Material);
        }

        protected override void Dispose(bool disposing)
        {
            CoreUtils.Destroy(m_Material);
        }
#endif
    }
}
