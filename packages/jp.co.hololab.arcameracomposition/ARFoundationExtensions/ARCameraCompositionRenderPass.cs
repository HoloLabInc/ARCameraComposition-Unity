using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.XR.ARFoundation
{
    public class ARCameraCompositionRenderPass : ScriptableRenderPass
    {
        private readonly ProfilingSampler compositionProfilingSampler = new ProfilingSampler("ARCameraComposition");
        private readonly Material material;

        private RTHandle cameraColorTarget;
        private bool invertCulling;
        private float opacity;
        private Material backgroundMaterial;
        private RTHandle backgroundTarget;

        public ARCameraCompositionRenderPass(Material material)
        {
            this.material = material;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public void SetRenderTarget(RTHandle colorHandle)
        {
            cameraColorTarget = colorHandle;
        }

        public void SetUp(ARCameraBackground cameraBackground, bool invertCulling, float opacity)
        {
            backgroundMaterial = cameraBackground.material;
            this.invertCulling = invertCulling;
            this.opacity = opacity;
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            ConfigureTarget(cameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                return;
            }

            if (backgroundMaterial == null)
            {
                return;
            }

            var cameraData = renderingData.cameraData;
            if (cameraData.camera.cameraType != CameraType.Game)
            {
                return;
            }

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;
            RenderingUtils.ReAllocateIfNeeded(ref backgroundTarget, desc, FilterMode.Point,
                                                TextureWrapMode.Clamp, name: "_BackgroundTarget");

            CommandBuffer cmd = CommandBufferPool.Get();
            using (new ProfilingScope(cmd, compositionProfilingSampler))
            {
                material.SetFloat("_Opacity", opacity);

                CoreUtils.SetRenderTarget(cmd, backgroundTarget);
                cmd.SetInvertCulling(invertCulling);
                cmd.SetViewProjectionMatrices(Matrix4x4.identity, ProjectionMatrix);
                cmd.DrawMesh(Mesh, Matrix4x4.identity, backgroundMaterial);
                material.SetTexture(Shader.PropertyToID("_ARCameraTex"), backgroundTarget);

                Blitter.BlitCameraTexture(cmd, cameraColorTarget, cameraColorTarget, material, 0);
            }
            context.ExecuteCommandBuffer(cmd);
            cmd.Clear();

            CommandBufferPool.Release(cmd);
        }


        private Matrix4x4 ProjectionMatrix => ARCameraBackgroundRenderingUtils.afterOpaquesOrthoProjection;

        private Mesh Mesh => ARCameraBackgroundRenderingUtils.fullScreenFarClipMesh;
    }
}
