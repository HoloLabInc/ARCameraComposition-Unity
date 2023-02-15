using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
#if URP_PRESENT
using UnityEngine.Rendering.Universal;
#endif
using UnityEngine.XR.ARFoundation;

namespace HoloLab.ARCameraComposition
{
    public class ARCameraCompositionOpacityConfigurator : MonoBehaviour
    {
        private ARCameraCompositionRendererFeature rendererFeature;

        private void Awake()
        {
            rendererFeature = GetRendererFeature();
        }

        public float GetOpacity()
        {
            return rendererFeature.Opacity;
        }

        public void SetOpacity(float opacity)
        {
            if (rendererFeature != null)
            {
                rendererFeature.Opacity = opacity;
            }
        }

        private static ARCameraCompositionRendererFeature GetRendererFeature()
        {
#if URP_PRESENT
            var renderPipelineAsset = GraphicsSettings.renderPipelineAsset;
            var urpAsset = renderPipelineAsset as UniversalRenderPipelineAsset;
            if (urpAsset == null)
            {
                return null;
            }

            var scriptableRenderer = urpAsset.scriptableRenderer;
            var scriptableRendererFeatures = GetScriptableRendererFeature(scriptableRenderer);
            var feature = scriptableRendererFeatures.OfType<ARCameraCompositionRendererFeature>()
                .FirstOrDefault();

            return feature;
#else
            return null;
#endif
        }

#if URP_PRESENT
        private static List<ScriptableRendererFeature> GetScriptableRendererFeature(ScriptableRenderer scriptableRenderer)
        {
            var type = typeof(ScriptableRenderer);
            var propertyInfo = type.GetField("m_RendererFeatures", BindingFlags.Instance | BindingFlags.NonPublic);
            return (List<ScriptableRendererFeature>)propertyInfo.GetValue(scriptableRenderer);
        }
#endif
    }
}