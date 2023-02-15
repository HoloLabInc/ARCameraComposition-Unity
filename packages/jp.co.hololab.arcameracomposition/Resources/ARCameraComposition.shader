Shader "AR Camera Composition/AR Camera Composition"
{
    Properties
    {
        _Opacity("Opacity", Float) = 0.9
        [HideInInspector]
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "RenderPipeline"="UniversalPipeline"
        }
        LOD 100

        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "ForwardLit"

            Tags
            {
                "LightMode"="UniversalForward"
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_ARCameraTex);
            SAMPLER(sampler_ARCameraTex);

            CBUFFER_START(UnityPerMaterial)
            float4 _MainTex_ST;
            half _Opacity;
            CBUFFER_END

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float4 cameraColor = SAMPLE_TEXTURE2D(_ARCameraTex, sampler_ARCameraTex, i.uv);

                half mainAlpha = mainColor.a * _Opacity;
                half cameraAlpha = 1 - mainAlpha;

                float4 col = mainColor * mainAlpha + cameraColor * cameraAlpha;
                col.a = 1;

                return col;
            }
            ENDHLSL
        }
    }
}