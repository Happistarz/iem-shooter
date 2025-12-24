Shader "Custom/CelShaderFoliageWave"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Glossiness", Float) = 32
        [HDR]
        _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0, 1)) = 0.7
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.5
        
        _WaveAmplitude ("Wave Amplitude", Float) = 0.1
        _WaveFrequency ("Wave Frequency", Float) = 1.0
        _WaveSpeed ("Wave Speed", Float) = 1.0

        [Header(Dissolve)]
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.0
        [HDR] _DissolveColor ("Dissolve Edge Color", Color) = (1, 0.5, 0, 1)
        _DissolveEdgeWidth ("Dissolve Edge Width", Range(0.0, 1.0)) = 0.1
        _DissolveHeightMax ("Dissolve Height Max", Float) = 5.0
        _DissolveNoiseTex ("Dissolve Noise Texture", 2D) = "white" {}
        _DissolveNoiseScale ("Dissolve Noise Scale", Float) = 10.0
        _DissolveNoiseStrength ("Dissolve Noise Strength", Float) = 1.0
        _TimeOffset ("Time Offset", Float) = 0.0
    }
    SubShader
    {
        Tags
        {
            "LightMode"="ForwardBase"
            "PassFlags"="OnlyDirectional"
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase

            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float3 objPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Glossiness;

            float4 _RimColor;
            float _RimPower;
            float _RimThreshold;
            
            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;

            float _DissolveAmount;
            float4 _DissolveColor;
            float _DissolveEdgeWidth;
            float _DissolveHeightMax;
            
            sampler2D _DissolveNoiseTex;
            float4 _DissolveNoiseTex_ST;
            float _DissolveNoiseScale;
            float _DissolveNoiseStrength;
            float _TimeOffset;

            v2f vert(appdata v)
            {
                v2f o;
                
                float time = _Time.y + _TimeOffset;
                float wave = sin(_WaveFrequency * v.vertex.y + _WaveSpeed * time) * _WaveAmplitude;
                
                v.vertex.x += wave * 0.5;
                v.vertex.z += wave * 0.3;

                o.objPos = v.vertex.xyz;

                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Dissolve Logic
                float noise = tex2D(_DissolveNoiseTex, i.uv * _DissolveNoiseScale).r;
                
                // Adjust range to ensure full dissolve even with noise
                float minHeight = -_DissolveNoiseStrength - 0.5;
                float cutHeight = lerp(_DissolveHeightMax, minHeight, _DissolveAmount);
                
                // Perturb the height check with noise
                float noisyHeight = i.objPos.y - (noise * _DissolveNoiseStrength);
                
                if (noisyHeight > cutHeight) discard;

                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);

                // Calculate lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(normal, lightDir));

                // Cel shading
                float intensity = ceil(NdotL * _Glossiness) / _Glossiness;

                // Rim lighting
                float rimDot = 1.0 - dot(normal, viewDir);
                float rimIntensity = rimDot * pow(1.0 - NdotL, _RimThreshold);
                rimIntensity = smoothstep(_RimPower - 0.01, _RimPower + 0.01, rimIntensity);
                float4 rim = _RimColor * rimIntensity;

                float4 sample = tex2D(_MainTex, i.uv);
                
                float4 finalColor = (sample * intensity + rim) * _Color;

                // Apply Dissolve Edge
                float edgeFactor = 1.0 - saturate((cutHeight - noisyHeight) / _DissolveEdgeWidth);
                finalColor.rgb = lerp(finalColor.rgb, _DissolveColor.rgb, edgeFactor);

                return finalColor;
            }
            ENDCG
        }
    }
}