Shader "AR Camera Composition/AR Camera Composition"
{
    Properties
    {
        _Opacity("Opacity", Float) = 0.9
        // [HideInInspector]
        // _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Cull Off ZWrite Off

        Pass
        {
            Name "ARCameraCompositionPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            // The Blit.hlsl file provides the vertex shader (Vert),
            // input structure (Attributes) and output strucutre (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D_X(_ARCameraTex);
            SAMPLER(sampler_ARCameraTex);

            //TEXTURE2D_X(_CameraOpaqueTexture);
            //SAMPLER(sampler_CameraOpaqueTexture);

            // TEXTURE2D_X(_MainTex);
            // SAMPLER(sampler_MainTex);

            TEXTURE2D_X(_MainCameraTex);
            SAMPLER(sampler_MainCameraTex);

            half _Opacity;

            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                // float4 cameraColor = SAMPLE_TEXTURE2D_X(_ARCameraTex, sampler_ARCameraTex, input.texcoord);
                // float4 mainColor = SAMPLE_TEXTURE2D_X(_MainTex, sampler_MainTex, input.texcoord);

                 float4 cameraColor = SAMPLE_TEXTURE2D_X(_ARCameraTex, sampler_ARCameraTex, input.texcoord);
                // float4 cameraColor = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, input.texcoord);
                float4 mainColor = SAMPLE_TEXTURE2D_X(_MainCameraTex, sampler_MainCameraTex, input.texcoord);

                // half mainAlpha = mainColor.a * _Opacity;
                half mainAlpha = _Opacity;
                half cameraAlpha = 1 - mainAlpha;

                float4 col = mainColor * mainAlpha + cameraColor * cameraAlpha;
                col.a = 1;
                return col;
            }
            ENDHLSL
        }
    }
}