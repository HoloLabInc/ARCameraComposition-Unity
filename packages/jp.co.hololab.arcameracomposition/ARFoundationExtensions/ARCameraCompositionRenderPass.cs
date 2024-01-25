using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.XR.ARFoundation
{
    public class ARCameraCompositionRenderPass : ScriptableRenderPass
    {
        ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ARCameraComposition");
        Material m_Material;
        RTHandle m_CameraColorTarget;
        // float m_Intensity;
        float m_opacity;
        private Material m_BackgroundMaterial;

        public ARCameraCompositionRenderPass(Material material)
        {
            m_Material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorHandle, float opacity)
        {
            m_CameraColorTarget = colorHandle;
            m_opacity = opacity;
            // RenderingUtils.ReAllocateIfNeeded(ref m_CopiedColor, colorCopyDescriptor, name: "_FullscreenPassColorCopy");
        }

        public void Setup(ARCameraBackground cameraBackground)
        {
            m_BackgroundMaterial = cameraBackground.material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(m_CameraColorTarget);
        }

        private RTHandle m_Handle;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            if (cameraData.camera.cameraType != CameraType.Game)
                return;

            if (m_Material == null)
                return;


            if (m_BackgroundMaterial == null)
            {
                return;
            }

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            // Then using RTHandles, the color and the depth properties must be separate
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref m_Handle, desc, FilterMode.Point,
                                                TextureWrapMode.Clamp, name: "_CustomPassHandle");

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // cmd.SetGlobalTexture("_ARCameraTex", m_BackgroundMaterial);
                //Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
                //Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_Handle, m_Material, 0);
                Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_Handle, 0);
                cmd.SetGlobalTexture("_MainCameraTex", m_Handle);

                cmd.SetViewProjectionMatrices(Matrix4x4.identity, projectionMatrix);
                cmd.DrawMesh(mesh, Matrix4x4.identity, m_BackgroundMaterial);

                m_Material.SetFloat("_Opacity", m_opacity);
                Blitter.BlitCameraTexture(cmd, m_Handle, m_CameraColorTarget, m_Material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }

        protected Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

        /// <inheritdoc />
        protected Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
    }
}
