Shader "Unlit/TractorBeamShader"
{
    Properties
    {
        [Header(Gradient Colors)]
        _TopColor ("Top Color (Start)", Color) = (1,1,1,1)
        _BottomColor ("Bottom Color (End)", Color) = (0,1,1,1)
        
        [Header(Fresnel)]
        _FresnelColor ("Fresnel Color", Color) = (1,1,1,1)
        _FresnelPower ("Fresnel Power", Range(0, 10)) = 3.0
        _FresnelThreshold ("Fresnel Threshold", Range(0, 1)) = 0.5
        
        [Header(Scanline)]
        _ScanlineTex ("Scanline Texture", 2D) = "black" {}
        _ScanlineSpeed ("Scanline Speed", Range(-5, 5)) = 1.0
        _ScanlineColor ("Scanline Color", Color) = (1,1,1,1)

        [Header(Rings)]
        _RingColor ("Ring Color", Color) = (1,1,1,1)
        _RingSpeed ("Ring Speed", Range(-10, 10)) = 2.0
        _RingDensity ("Ring Density", Range(0, 20)) = 5.0
        _RingThickness ("Ring Thickness", Range(0.01, 0.5)) = 0.05
        
        [Header(Shared Settings)]
        _BottomWidth ("Bottom Width Scale", Range(0.0, 1.0)) = 1.0
        _GlobalAlpha ("Global Alpha", Range(0.0, 1.0)) = 1.0
        _RandomOffset ("Random Offset", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        CGINCLUDE
            #include "UnityCG.cginc"

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
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float3 objNormal : TEXCOORD3;
            };

            // Gradient Colors
            fixed4 _TopColor;
            fixed4 _BottomColor;

            // Fresnel
            fixed4 _FresnelColor;
            float _FresnelPower;
            float _FresnelThreshold;

            // Scanline
            sampler2D _ScanlineTex;
            float4 _ScanlineTex_ST;
            float _ScanlineSpeed;
            fixed4 _ScanlineColor;

            // Rings
            fixed4 _RingColor;
            float _RingSpeed;
            float _RingDensity;
            float _RingThickness;

            // Shared Settings
            float _BottomWidth;
            float _GlobalAlpha;
            float _RandomOffset;

            v2f vert (appdata v)
            {
                v2f o;
                o.objNormal = v.normal;
                
                float4 pos = v.vertex;
                
                float scale = lerp(_BottomWidth, 1.0, v.uv.y);
                pos.xz *= scale;
                
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = v.uv;
                
                float3 worldPos = mul(unity_ObjectToWorld, pos).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = UnityWorldSpaceViewDir(worldPos);
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (i.objNormal.y < -0.5) discard;

                float time = _Time.y + _RandomOffset;

                // BASE GRADIENT
                fixed4 col = lerp(_BottomColor, _TopColor, i.uv.y);
                
                // SCANLINE
                float2 scanUV = i.uv * _ScanlineTex_ST.xy + _ScanlineTex_ST.zw;
                scanUV.y += time * _ScanlineSpeed;
                fixed4 scan = tex2D(_ScanlineTex, float2(0.5, scanUV.y));
                col.rgb += scan.rgb * _ScanlineColor.rgb * _ScanlineColor.a;

                // FRESNEL
                float3 normal = normalize(i.normal);
                float3 viewDir = normalize(i.viewDir);
                float NdotV = saturate(dot(normal, viewDir));
                float fresnel = pow(1.0 - NdotV, _FresnelPower);
                float edge = step(_FresnelThreshold, fresnel);
                
                col.rgb = lerp(col.rgb, _FresnelColor.rgb, edge);
                col.a = saturate(col.a + edge);

                // RINGS
                float ringPos = i.uv.y * _RingDensity + time * _RingSpeed;
                float ringWave = sin(ringPos * 3.14159) * 0.5 + 0.5;
                float ringThreshold = 1.0 - _RingThickness;
                
                if (ringWave > ringThreshold)
                {
                    float profile = (ringWave - ringThreshold) / (1.0 - ringThreshold);
                    
                    fixed4 ringCol = _RingColor;
                    
                    ringCol.rgb += fixed3(1,1,1) * pow(profile, 16.0) * 0.8; 
                    
                    col.rgb = lerp(col.rgb, ringCol.rgb, ringCol.a);
                    col.a = max(col.a, 1.0);
                }
                
                col.a *= _GlobalAlpha;
                
                return col;
            }
        ENDCG

        // BACK FACES
        Pass
        {
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }

        // FRONT FACES
        Pass
        {
            Cull Back
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            ENDCG
        }
    }
}
