
Shader "FaceCanvas/FacePaint"
{
    Properties
    {
        _PaintTex ("Paint Texture (RGBA)", 2D) = "white" {}
        _Tint ("Tint", Color) = (1,1,1,1)
        _Opacity ("Opacity", Range(0,1)) = 0.9
        _Glow ("Glow", Range(0,3)) = 0
        _Sparkle ("Sparkle", Range(0,1)) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "RenderPipeline"="UniversalPipeline"
        }

        Pass
        {
            Name "Paint"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_PaintTex);
            SAMPLER(sampler_PaintTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _PaintTex_ST;
                half4 _Tint;
                half _Opacity;
                half _Glow;
                half _Sparkle;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS =
                    TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = TRANSFORM_TEX(IN.uv, _PaintTex);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                half4 paint = SAMPLE_TEXTURE2D(
                    _PaintTex,
                    sampler_PaintTex,
                    IN.uv
                );

                half3 rgb = paint.rgb * _Tint.rgb * (1.0 + _Glow);

                float h = frac(
                    sin(dot(
                        floor(IN.uv * 300.0),
                        float2(12.9898, 78.233)
                    )) * 43758.5453
                );

                float twinkle = step(0.97, h) *
                    (0.5 + 0.5 *
                    sin(_Time.y * 6.0 + h * 50.0));

                rgb += twinkle * _Sparkle * paint.a;

                half a = saturate(
                    paint.a * _Tint.a * _Opacity
                );

                return half4(rgb, a);
            }

            ENDHLSL
        }
    }
}
