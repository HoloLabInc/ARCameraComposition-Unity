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
        private bool m_InvertCulling;
        private float m_Opacity;
        private Material m_BackgroundMaterial;
        // private RTHandle m_Handle;
        private RTHandle m_Handle2;

        public ARCameraCompositionRenderPass(Material material)
        {
            m_Material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetRenderTarget(RTHandle colorHandle)
        {
            m_CameraColorTarget = colorHandle;
        }

        public void SetUp(ARCameraBackground cameraBackground, bool invertCulling, float opacity)
        {
            m_BackgroundMaterial = cameraBackground.material;
            m_InvertCulling = invertCulling;
            m_Opacity = opacity;
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
            //RenderingUtils.ReAllocateIfNeeded(ref m_Handle, desc, FilterMode.Point,
                                                //TextureWrapMode.Clamp, name: "_CustomPassHandle");
            RenderingUtils.ReAllocateIfNeeded(ref m_Handle2, desc, FilterMode.Point,
                                                TextureWrapMode.Clamp, name: "_CustomPassHandle2");

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                m_Material.SetFloat("_Opacity", m_Opacity);

                CoreUtils.SetRenderTarget(cmd, m_Handle2);
                cmd.SetInvertCulling(m_InvertCulling);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, ProjectionMatrix);
                cmd.DrawMesh(Mesh, Matrix4x4.identity, m_BackgroundMaterial);
                m_Material.SetTexture(Shader.PropertyToID("_ARCameraTex"), m_Handle2);

                Blitter.BlitCameraTexture(cmd, m_CameraColorTarget, m_CameraColorTarget, m_Material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }


        private Matrix4x4 ProjectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

        private Mesh Mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
    }
}
