Shader "Universal Render Pipeline/GreenVolumeDistortion"
{
    Properties
    {
        _MainTex("Screen Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _DistortionStrength("Distortion Strength", Range(0,0.2)) = 0.05
        _VolumeColor("Volume Color", Color) = (0,1,0,0.3)
        _Alpha("Transparency", Range(0,1)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardUnlit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_NoiseTex);
            SAMPLER(sampler_NoiseTex);

            float _DistortionStrength;
            float4 _VolumeColor;
            float _Alpha;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // ノイズによるUVオフセット
                float2 noiseUV = IN.uv + float2(cos(_Time.y * 0.5), sin(_Time.y * 0.3));
                float2 offset = SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, noiseUV).rg * _DistortionStrength;

                // 背景（_MainTex）を歪ませて取得
                half4 bgColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv + offset);

                // 緑のボリュームをブレンド
                half4 finalColor = lerp(bgColor, _VolumeColor, _VolumeColor.a);

                // 全体の透明度を適用
                finalColor.a *= _Alpha;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
