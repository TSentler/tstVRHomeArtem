
Shader "Custom/URP/HolographicGlitch"
{
    Properties
    {
        _ColorA ("Cyan", Color) = (0, 0.35, 0.8, 1)
        _ColorB ("Magenta", Color) = (0.65, 0, 1, 1)

        _Speed ("Animation Speed", Range(0, 5)) = 1.5
        _FresnelPower ("Fresnel Power", Range(0.1, 8)) = 2.5
        _ScanSpeed ("Scan Speed", Range(0, 20)) = 8
        _ScanDensity ("Scan Density", Range(1, 200)) = 120
        _Glitch ("Glitch Strength", Range(0, 0.2)) = 0.025

        _StripeContrast ("Stripe Contrast", Range(0, 1)) = 0.8
        _Glow ("Edge Glow", Range(0, 2)) = 0.35
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float4 _ColorA;
                float4 _ColorB;
                float _Speed;
                float _FresnelPower;
                float _ScanSpeed;
                float _ScanDensity;
                float _Glitch;
                float _StripeContrast;
                float _Glow;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normalWS : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
            };

            float hash(float n)
            {
                return frac(sin(n) * 43758.5453);
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 worldPos =
                    TransformObjectToWorld(IN.positionOS.xyz);

                float t = _Time.y * _Speed;

                // Digital glitch
                float glitch = hash(floor(t * 8)) - 0.5;

                worldPos.x += glitch * _Glitch *
                    sin(worldPos.y * 30 + t * 10);

                OUT.positionHCS =
                    TransformWorldToHClip(worldPos);

                OUT.worldPos = worldPos;

                OUT.normalWS =
                    TransformObjectToWorldNormal(IN.normalOS);

                OUT.viewDir =
                    GetWorldSpaceViewDir(worldPos);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float t = _Time.y * _Speed;

                float3 normal = normalize(IN.normalWS);
                float3 viewDir = normalize(IN.viewDir);

                // Fresnel edge effect
                float fresnel = pow(
                    1 - saturate(dot(normal, viewDir)),
                    _FresnelPower
                );

                // Color gradient
                float rainbow = sin(
                    dot(IN.worldPos, float3(1, 2, 1))
                    * 3 + t * 2
                );

                float3 color = lerp(
                    _ColorA.rgb,
                    _ColorB.rgb,
                    rainbow * 0.5 + 0.5
                );

                // Scanlines
                float scan = sin(
                    IN.worldPos.y * _ScanDensity
                    - t * _ScanSpeed
                );

                scan = smoothstep(-0.1, 0.5, scan);

                // Dark gaps between stripes
                float stripe = lerp(
                    1.0 - _StripeContrast,
                    1.0,
                    scan
                );

                color *= stripe;

                // Subtle edge glow
                color += fresnel * _Glow;

                // Pulse
                float pulse = 0.85 + 0.15 * sin(t * 3);
                color *= pulse;

                // Prevent overexposure
                color = saturate(color);

                // Transparency
                float alpha = saturate(
                    0.3 + fresnel * 0.5 + scan * 0.1
                );

                return half4(color, alpha);
            }

            ENDHLSL
        }
    }
}
