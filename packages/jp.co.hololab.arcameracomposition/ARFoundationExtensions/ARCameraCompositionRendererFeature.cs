using UnityEngine;
using UnityEngine.Rendering;
#if MODULE_URP_ENABLED
using UnityEngine.Rendering.Universal;

#elif MODULE_LWRP_ENABLED
using UnityEngine.Rendering.LWRP;
#else
using ScriptableRendererFeature = UnityEngine.ScriptableObject;
#endif

namespace UnityEngine.XR.ARFoundation
{
    /// <summary>
    /// A render feature for composite AR content and camera image for AR devies.
    /// </summary>
    public class ARCameraCompositionRendererFeature : ScriptableRendererFeature
    {
        [SerializeField]
        [Range(0, 1)]
        private float opacity = 0.9f;

        public float Opacity { set; get; }

#if MODULE_URP_ENABLED || MODULE_LWRP_ENABLED
        /// <summary>
        /// The scriptable render pass to be added to the renderer when the camera background is to be rendered.
        /// </summary>
        CustomRenderPass m_ScriptablePass;

        /// <summary>
        /// Create the scriptable render pass.
        /// </summary>
        public override void Create()
        {
            Opacity = opacity;
            m_ScriptablePass = new CustomRenderPass(RenderPassEvent.AfterRenderingPostProcessing);
        }

        /// <summary>
        /// Add the background rendering pass when rendering a game camera with an enabled AR camera background component.
        /// </summary>
        /// <param name="renderer">The sriptable renderer in which to enqueue the render pass.</param>
        /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            Camera currentCamera = renderingData.cameraData.camera;
            if ((currentCamera != null) && (currentCamera.cameraType == CameraType.Game))
            {
                ARCameraBackground cameraBackground = currentCamera.gameObject.GetComponent<ARCameraBackground>();

                if ((cameraBackground != null) && cameraBackground.backgroundRenderingEnabled
                                               && (cameraBackground.material != null))
                {
                    bool invertCulling = cameraBackground.GetComponent<ARCameraManager>()?.subsystem?.invertCulling ??
                                         false;
                    m_ScriptablePass.Setup(cameraBackground.material, Opacity, invertCulling);
                    renderer.EnqueuePass(m_ScriptablePass);
                }
            }
        }

        /// <summary>
        /// The custom render pass to render the camera background.
        /// </summary>
        class CustomRenderPass : ScriptableRenderPass
        {
            /// <summary>
            /// The name for the custom render pass which will be display in graphics debugging tools.
            /// </summary>
            const string k_CustomRenderPassName = "AR Camera Composition Pass (URP)";

            /// <summary>
            /// The material used for rendering the device background using the camera video texture and potentially
            /// other device-specific properties and textures.
            /// </summary>
            Material m_BackgroundMaterial;

            /// <summary>
            /// Whether the culling mode should be inverted.
            /// ([CommandBuffer.SetInvertCulling](https://docs.unity3d.com/ScriptReference/Rendering.CommandBuffer.SetInvertCulling.html)).
            /// </summary>
            bool m_InvertCulling;

            Material m_MrcBackgroundMaterial;

            RenderTargetHandle m_TemporaryColorTexture;

            RenderTargetHandle m_TemporaryARCameraTexture;

            private static readonly int OpacityID = Shader.PropertyToID("_Opacity");

            /// <summary>
            /// Constructs the background render pass.
            /// </summary>
            /// <param name="renderPassEvent">The render pass event when this pass should be rendered.</param>
            public CustomRenderPass(RenderPassEvent renderPassEvent)
            {
                this.renderPassEvent = renderPassEvent;
                m_TemporaryColorTexture.Init("_TemporaryColorTexture");
                m_TemporaryARCameraTexture.Init("_TemporaryARCameraTexture");
            }

            public void Setup(Material backgroundMaterial, float opacity, bool invertCulling)
            {
                m_BackgroundMaterial = backgroundMaterial;
                var shader = Shader.Find("AR Camera Composition/AR Camera Composition");

                m_MrcBackgroundMaterial = new Material(shader);
                m_MrcBackgroundMaterial.SetFloat(OpacityID, opacity);
                m_InvertCulling = invertCulling;
            }

            /// <summary>
            /// Configure the render pass by configuring the render target and clear values.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for configuration.</param>
            /// <param name="renderTextureDescriptor">The descriptor of the target render texture.</param>
            public override void Configure(CommandBuffer commandBuffer, RenderTextureDescriptor renderTextureDescriptor)
            {
                commandBuffer.GetTemporaryRT(m_TemporaryColorTexture.id, renderTextureDescriptor);
                ConfigureTarget(m_TemporaryColorTexture.Identifier());
                ConfigureClear(ClearFlag.All, Color.clear);

                commandBuffer.GetTemporaryRT(m_TemporaryARCameraTexture.id, renderTextureDescriptor);
                ConfigureTarget(m_TemporaryARCameraTexture.Identifier());
                ConfigureClear(ClearFlag.All, Color.black);
            }

            /// <summary>
            /// Execute the commands to render the camera background.
            /// </summary>
            /// <param name="context">The render context for executing the render commands.</param>
            /// <param name="renderingData">Additional rendering data about the current state of rendering.</param>
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                var cmd = CommandBufferPool.Get(k_CustomRenderPassName);
                cmd.BeginSample(k_CustomRenderPassName);

                ARCameraBackground.AddBeforeBackgroundRenderHandler(cmd);
                cmd.SetInvertCulling(m_InvertCulling);

                var cameraColorTarget = renderingData.cameraData.renderer.cameraColorTarget;

                cmd.Blit(null, m_TemporaryARCameraTexture.id, m_BackgroundMaterial);
                cmd.SetGlobalTexture("_ARCameraTex", m_TemporaryARCameraTexture.id);

                cmd.Blit(cameraColorTarget, m_TemporaryColorTexture.id, m_MrcBackgroundMaterial);
                cmd.Blit(m_TemporaryColorTexture.id, cameraColorTarget);

                cmd.EndSample(k_CustomRenderPassName);
                context.ExecuteCommandBuffer(cmd);

                CommandBufferPool.Release(cmd);
            }

            /// <summary>
            /// Clean up any resources for the render pass.
            /// </summary>
            /// <param name="commandBuffer">The command buffer for frame cleanup.</param>
            public override void FrameCleanup(CommandBuffer commandBuffer)
            {
                commandBuffer.ReleaseTemporaryRT(m_TemporaryColorTexture.id);
                commandBuffer.ReleaseTemporaryRT(m_TemporaryARCameraTexture.id);
            }
        }
#endif // MODULE_URP_ENABLED || MODULE_LWRP_ENABLED
    }
}