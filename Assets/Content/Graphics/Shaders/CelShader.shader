Shader "Unlit/CelShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Glossiness ("Glossiness", Float) = 32
        
        [HDR] _RimColor ("Rim Color", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0, 1)) = 0.7
        _RimThreshold ("Rim Threshold", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags
        {
            "LightMode"="ForwardBase"
            "PassFlags"="OnlyDirectional"
        }
        LOD 100

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
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            float _Glossiness;

            float4 _RimColor;
            float _RimPower;
            float _RimThreshold;

            v2f vert(appdata v)
            {
                v2f o;

                o.viewDir = WorldSpaceViewDir(v.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal = UnityObjectToWorldNormal(v.normal);

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);

                // Calculate lighting
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = max(0, dot(normal, lightDir));

                // glossiness
                float intensity = ceil(NdotL * _Glossiness) / _Glossiness;

                // Rim lighting
                float rimDot = 1.0 - dot(normal, viewDir);
                float rimIntensity = rimDot * pow(1.0 - NdotL, _RimThreshold);
                rimIntensity = smoothstep(_RimPower - 0.01, _RimPower + 0.01, rimIntensity);
                float4 rim = _RimColor * rimIntensity;

                float4 sample = tex2D(_MainTex, i.uv);
                
                return (sample * intensity + rim) * _Color;
            }
            ENDCG
        }
    }
}