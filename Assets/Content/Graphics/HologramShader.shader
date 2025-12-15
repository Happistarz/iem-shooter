//Shader "Unlit/HologramShader"
//{
//    Properties
//    {
//        _MainTex ("Texture", 2D) = "white" {}
//        _FresnelPower ("Fresnel Power", Range(1, 5)) = 4.0
//        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
//        _HologramLineTexture ("Hologram Line Texture", 2D) = "white" {}
//        _HologramLineComplexTexture ("Hologram Line Complex Texture", 2D) = "white" {}
//        _HologramLineSpeed ("Hologram Line Speed", Float) = 1.0
//    }
//    SubShader
//    {
//        Tags
//        {
//            "Queue"="Transparent"
//            "RenderType"="Transparent"
//            "BuiltInMaterialType"="Unlit"
//        }
//        Cull Back
//        ZWrite Off
//        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
//        ZTest LEqual
//        ColorMask RGB
//
//        Pass
//        {
//            CGPROGRAM
//            #pragma vertex vert
//            #pragma fragment frag
//            #pragma target 3.0
//            #pragma mutli_compile_fwdbase
//            #pragma multi_compile_instancing
//
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float3 normal : NORMAL;
//                float2 uv : TEXCOORD0;
//            };
//
//            struct v2f
//            {
//                float4 vertex : SV_POSITION;
//                float3 viewDir : TEXCOORD1;
//                float3 worldPos : TEXCOORD2;
//                float3 screenPos : TEXCOORD3;
//                float3 normal : NORMAL;
//                float2 uv : TEXCOORD0;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//            
//            sampler2D _HologramLineTexture;
//            sampler2D _HologramLineComplexTexture;
//            float _HologramLineSpeed;
//            
//            float4 _FresnelColor;
//            float _FresnelPower;
//
//            v2f vert(appdata v)
//            {
//                v2f o;
//                o.vertex = UnityObjectToClipPos(v.vertex);
//                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
//                o.viewDir = _WorldSpaceCameraPos - o.worldPos;
//                o.screenPos = UnityObjectToClipPos(v.vertex).xyz / UnityObjectToClipPos(v.vertex).w;
//                o.normal = mul((float3x3)unity_ObjectToWorld, v.normal);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//
//                return o;
//            }
//
//            fixed4 frag(v2f i) : SV_Target
//            {
//                float3 normal = normalize(i.normal);
//                float3 viewDir = normalize(i.viewDir);
//
//                // Fresnel effect
//                // float fresnel = pow(1.0 - saturate(dot(normal, viewDir)), _FresnelPower);
//                //
//                // float4 sample = tex2D(_MainTex, i.uv);
//                //
//                // float3 fresnelColor = _FresnelColor * fresnel;
//                // float3 finalColor = sample.rgb + fresnelColor;
//                // return float4(finalColor, fresnel);
//
//                // Scanning lines effect
//                // tiling and offset for the line textures
//                float2 lineUV = i.screenPos.xy * 10.0; // Adjust tiling here
//                lineUV.y += _Time.y * _HologramLineSpeed;
//                float lineSample = tex2D(_HologramLineTexture, lineUV).r;
//
//                float4 sample = tex2D(_MainTex, i.uv);
//
//                float3 finalColor = sample.rgb * lineSample;
//                return float4(finalColor, lineSample);
//            }
//            ENDCG
//        }
//    }
//}
Shader "Unlit/HologramShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Texture", 2D) = "white" {}
        _AlphaTexture ("Alpha Texture", 2D) = "white" {}
        _Scale ("Alpha Tiling", Float) = 3
        _ScrollSpeedV ("Alpha Scroll Speed", Range(0, 5.0)) = 1.0
        _GlowIntensity ("Glow Intensity", Range(0.01, 1.0)) = 0.5
        _GlitchSpeed ("Glitch Speed", Range(0, 50)) = 50.0
        _GlitchIntensity ("Glitch Intensity", Range(0, 1.0)) = 0
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "BuiltInMaterialType"="Unlit"
            "IgnoreProjector"="True"
        }
        Lighting Off
        ZWrite On
        Blend SrcAlpha One
        Cull Back

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma mutli_compile_fwdbase
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float3 worldNormal : NORMAL;
                float3 grabPos : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color, _MainTex_ST;
            sampler2D _MainTex, _AlphaTexture;
            half _Scale, _ScrollSpeedV, _GlowIntensity, _GlitchSpeed, _GlitchIntensity;

            v2f vert(appdata IN)
            {
                v2f OUT;
                IN.vertex.z += sin(_Time.y * _GlitchSpeed * 5 * IN.vertex.y) * _GlitchIntensity;
                OUT.position = UnityObjectToClipPos(IN.vertex);
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.grabPos = UnityObjectToViewPos(IN.vertex);
                OUT.grabPos.y += _Time * _ScrollSpeedV;
                OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                OUT.viewDir = normalize(UnityWorldSpaceViewDir(OUT.grabPos.xyz));

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                half dir = (dot(IN.grabPos, 1.0) + 1.0) * 0.5;

                fixed4 alphaColor = tex2D(_AlphaTexture, IN.grabPos.xy * _Scale);
                fixed4 pixel = tex2D(_MainTex, IN.uv);
                pixel.w = alphaColor.w;

                half rim = 1.0 - saturate(dot(normalize(IN.worldNormal), IN.viewDir));
                return pixel * _Color * (rim + _GlowIntensity);
            }
            ENDCG
        }
    }
}