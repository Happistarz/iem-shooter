Shader "Custom/TerrainModifierShader"
{
    Properties
    {
        [Header(Base Settings)]
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color Tint", Color) = (1,1,1,1)
        _Glossiness ("Smoothness", Float) = 32

        [Header(Corruption Effects)]
        _CorruptionAmount ("Corruption Amount", Range(0, 1)) = 0.0
        _NoiseTexture ("Noise Texture", 2D) = "white" {}
        _NoiseScale ("Noise Scale", Float) = 0.05
        [HDR] _CorruptionColor ("Corruption Color", Color) = (0.2,0.2,0.2,1)

        [Header(Corruption Edge)]
        [HDR] _EdgeColor ("Edge Color", Color) = (1,1,0,1)
        _EdgeThickness ("Edge Thickness", Range(0, 0.2)) = 0.05
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
            "LightMode"="ForwardBase"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
                float2 uv : TEXCOORD0;
                LIGHTING_COORDS(3,4)
            };

            // Main Texture and Color
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;

            // Lighting
            float _Glossiness;

            // Corruption Settings
            float _CorruptionAmount;
            float4 _CorruptionColor;

            // Dissolve Noise Settings
            sampler2D _NoiseTexture;
            float4 _NoiseTexture_ST;
            float _NoiseScale;

            // Dissolve Edge Settings
            float4 _EdgeColor;
            float _EdgeThickness;

            v2f vert(appdata IN)
            {
                v2f OUT;

                OUT.pos = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normal = UnityObjectToWorldNormal(IN.normal);
                OUT.worldPos = mul(unity_ObjectToWorld, IN.vertex).xyz;
                OUT.viewDir = WorldSpaceViewDir(OUT.pos);

                TRANSFER_VERTEX_TO_FRAGMENT(OUT);
                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float3 normal = normalize(IN.normal);
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);

                float NdotL = max(0, dot(normal, lightDir));

                float lightIntensity = ceil(NdotL * _Glossiness) / _Glossiness;

                // Apply attenuation
                float attenuation = LIGHT_ATTENUATION(IN);
                lightIntensity *= attenuation;

                float4 texColor = tex2D(_MainTex, IN.uv);
                float4 baseColor = texColor * lightIntensity * _Color;

                // Corruption Effect
                float2 noiseUV = IN.worldPos.xz * _NoiseScale * _NoiseTexture_ST.xy + _NoiseTexture_ST.zw;
                float noiseValue = tex2D(_NoiseTexture, noiseUV).r;

                float corruptionMask = step(noiseValue, _CorruptionAmount);
                
                float edgeMask = (step(noiseValue, _CorruptionAmount + _EdgeThickness) - corruptionMask) * step(0.001, _CorruptionAmount);

                // Convert to grayscale for corrupted effect
                float luminance = dot(texColor.rgb, float3(0.3, 0.59, 0.11));
                float4 corruptedColor = float4(luminance, luminance, luminance, 1) * _CorruptionColor * lightIntensity;
                
                float4 finalColor = lerp(baseColor, corruptedColor, corruptionMask);

                // Apply Edge Color
                finalColor.rgb += _EdgeColor.rgb * edgeMask;

                UNITY_APPLY_FOG(IN.fogCoord, finalColor);
                return finalColor;
            }
            ENDCG
        }
    }
    Fallback "Diffuse"
}