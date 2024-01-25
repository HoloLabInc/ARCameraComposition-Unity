using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.XR.ARFoundation
{
    public class ARCameraCompositionRenderPass : ScriptableRenderPass
    {
        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("ARCameraComposition");
        private Material m_Material;
        private RTHandle m_CameraColorTarget;
        private float m_opacity;
        private Material m_BackgroundMaterial;
        private RTHandle m_Handle;

        public ARCameraCompositionRenderPass(Material material)
        {
            m_Material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetTarget(RTHandle colorHandle, float opacity)
        {
            m_CameraColorTarget = colorHandle;
            m_opacity = opacity;
        }

        public void Setup(ARCameraBackground cameraBackground)
        {
            m_BackgroundMaterial = cameraBackground.material;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(m_CameraColorTarget);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (m_Material == null)
            {
                return;
            }

            if (m_BackgroundMaterial == null)
            {
                return;
            }

            var cameraData = renderingData.cameraData;
            if (cameraData.camera.cameraType != CameraType.Game)
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

        private Matrix4x4 projectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

        private Mesh mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
    }
}
