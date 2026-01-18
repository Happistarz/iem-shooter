Shader "Unlit/HologramShader"
{
    Properties
    {
        [Header(Shared Settings)]
        [Enum(UnityEngine.Rendering.CullMode)] _CullMode("Cull Mode", Float) = 2
        [Enum(On, 1, Off, 0)] _ZWriteMode("Z Write Mode", Float) = 0
        _ExternalGlitchActive("External Glitch Active", Float) = 0.0
        
        [Header(Color and Texture Settings)]
        _MainTint ("Main Tint", Color) = (21,114,178,1)
        _TextureTint("Texture Tint", Color) = (1,1,1,1)
        _TextureInfluence("Texture Influence", Range(0.0, 1.0)) = 0.0
        _GlowTint ("Glow Tint", Color) = (1,1,1,1)
        _RimPower ("Rim Power", Range(0.1, 10.0)) = 2.0
        _RimThreshold ("Rim Threshold", Range(0.0, 1.0)) = 0.9
        _Contrast ("Contrast", Range(0.1, 10.0)) = 2.0
        _BrightnessToAlpha ("Brightness To Alpha", Range(0.0, 1.0)) = 1.0
        [NoScaleOffset] _MainTex("Main Texture", 2D) = "white" {}
        _FlickerTex("Flicker Texture", 2D) = "white" {}
        _FlickerIntensity("Flicker Intensity", Range(0.0, 1.0)) = 0.1
        _FlickerSpeed("Flicker Speed", Range(1.0, 100.0)) = 3.0
        _FlickerRepeat("Flicker Repeat", Range(1.0, 100.0)) = 1.0

        [Header(Voxelization)]
        _VoxelSize("Voxel Size", Range(0.0, 0.1)) = 0.02
        _VoxelBlend("Voxel Blend", Range(0.0, 1.0)) = 0.5

        [Header(Glitch Effects)]
        _GlitchFrequency("Glitch Frequency", Range(0.0, 10.0)) = 2.0
        _GlitchProbability("Glitch Probability", Range(0.0, 1.0)) = 0.1
        _GlitchDuration("Glitch Duration", Range(0.0, 1.0)) = 0.5
        _GlitchIntensity("Glitch Intensity", Range(0.0, 1.0)) = 0.5
        _VertexGlitchAmount("Vertex Glitch Amount", Range(0.0, 1.0)) = 0.3
        _ColorGlitchAmount("Color Glitch Amount", Range(0.0, 1.0)) = 0.5

        [Header(Dissolve)]
        _DissolveTex("Dissolve Texture", 2D) = "white" {}
        _DissolveAmount("Dissolve Amount", Range(0.0, 1.0)) = 0.0
        _DissolveEdge("Dissolve Edge Width", Range(0.0, 0.3)) = 0.1

        [Header(Fade)]
        _VerticalFade("Vertical Fade", Range(0.0, 1.0)) = 1.0

        [Header(Scanlines)]
        _ScanLineCount("Scan Line Count", Float) = 250.0
        _ScanLineThickness("Scan Line Thickness", Range(0.0, 1.0)) = 0.5
        _ScanLineSpeed("Scan Line Speed", Range(-10.0, 10.0)) = 0.0

        [Header(Random)]
        _RandomOffset("Random Offset", Float) = 0.0

        [Header(Rainbow Effect)]
        [Toggle] _RainbowEnabled("Rainbow Enabled", Float) = 0
        _MousePos("Mouse Position (Screen UV)", Vector) = (0.5, 0.5, 0, 0)
        _RainbowIntensity("Rainbow Intensity", Range(0.0, 1.0)) = 0.3
        _RainbowWaveSpeed("Rainbow Wave Speed", Range(0.0, 5.0)) = 1.5
        _RainbowWaveScale("Rainbow Wave Scale", Range(0.1, 10.0)) = 3.0
        _RainbowDistortion("Rainbow Distortion", Range(0.0, 2.0)) = 0.5
        _RainbowRadius("Rainbow Radius", Range(0.1, 3.0)) = 1.2
        _RainbowSaturation("Rainbow Saturation", Range(0.0, 1.0)) = 0.7
        
    }

    SubShader
    {
        Tags 
        { 
            "Queue" = "Transparent" 
            "IgnoreProjector" = "True" 
            "RenderType" = "Transparent"
        }
        Cull [_CullMode]

        CGINCLUDE
            #include "UnityCG.cginc"
            #include "Lighting.cginc"

            // Shared Uniforms
            uniform float _ExternalGlitchActive;

            // Color and Texture
            uniform half4 _MainTint;
            uniform half4 _TextureTint;
            uniform float _TextureInfluence;
            uniform half4 _GlowTint;

            // Fresnel
            uniform float _RimPower;
            uniform float _RimThreshold;

            // Alpha
            uniform float _Contrast;
            uniform float _BrightnessToAlpha;

            // Main Texture
            uniform sampler2D _MainTex;
            uniform float4 _MainTex_ST;

            // Flicker
            uniform sampler2D _FlickerTex;
            uniform float _FlickerIntensity;
            uniform float _FlickerSpeed;
            uniform float _FlickerRepeat;

            // Voxelization
            uniform float _VoxelSize;
            uniform float _VoxelBlend;

            // Glitch
            uniform float _GlitchFrequency;
            uniform float _GlitchProbability;
            uniform float _GlitchDuration;
            uniform float _GlitchIntensity;
            uniform float _VertexGlitchAmount;
            uniform float _ColorGlitchAmount;

            // Dissolve
            uniform sampler2D _DissolveTex;
            uniform float4 _DissolveTex_ST;
            uniform float _DissolveAmount;
            uniform float _DissolveEdge;

            // Fade
            uniform float _VerticalFade;

            // Scanlines
            uniform float _ScanLineCount;
            uniform float _ScanLineThickness;
            uniform float _ScanLineSpeed;

            // Random
            uniform float _RandomOffset;

            // Rainbow Effect
            uniform float _RainbowEnabled;
            uniform float4 _MousePos;
            uniform float _RainbowIntensity;
            uniform float _RainbowWaveSpeed;
            uniform float _RainbowWaveScale;
            uniform float _RainbowDistortion;
            uniform float _RainbowRadius;
            uniform float _RainbowSaturation;

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 screenPos : VPOS;
                float4 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float3 viewDir : TEXCOORD1;
                float glitchActive : TEXCOORD2;
                float2 dissolveUV : TEXCOORD3;
                float3 worldPos : TEXCOORD4;
            };

            float hash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }

            float3 hsv2rgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
            }

            float simplexNoise2D(float2 p)
            {
                const float K1 = 0.366025404; // (sqrt(3)-1)/2
                const float K2 = 0.211324865; // (3-sqrt(3))/6
                
                float2 i = floor(p + (p.x + p.y) * K1);
                float2 a = p - i + (i.x + i.y) * K2;
                float2 o = a.x > a.y ? float2(1.0, 0.0) : float2(0.0, 1.0);
                float2 b = a - o + K2;
                float2 c = a - 1.0 + 2.0 * K2;
                
                float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);
                float3 n = h * h * h * h * float3(dot(a, hash2(i) - 0.5), dot(b, hash2(i + o) - 0.5), dot(c, hash2(i + 1.0) - 0.5));
                
                return dot(n, float3(70.0, 70.0, 70.0));
            }

            float3 calculateRainbowEffect(float2 screenUV, float3 worldPos, float time)
            {
                if (_RainbowEnabled < 0.5 || _RainbowIntensity < 0.001) return float3(0, 0, 0);
                
                float2 mouseUV = _MousePos.xy;
                
                float noiseTime = time * 0.3;
                float2 noiseCoord = screenUV * 3.0 + float2(noiseTime, noiseTime * 0.7);
                float distortionNoise = simplexNoise2D(noiseCoord) * _RainbowDistortion;
                float distortionNoise2 = simplexNoise2D(noiseCoord * 1.5 + 100.0) * _RainbowDistortion * 0.5;
                
                float2 diff = screenUV - mouseUV;
                float dist = length(diff) + distortionNoise * 0.1;
                
                float falloff = 1.0 - smoothstep(0.0, _RainbowRadius, dist);
                falloff = pow(falloff, 1.5);
                
                float xInfluence = mouseUV.x;
                float yInfluence = mouseUV.y;
                
                float3 colorShift;
                colorShift.r = xInfluence;
                colorShift.g = yInfluence;
                colorShift.b = 1.0 - (xInfluence * 0.5 + yInfluence * 0.5);
                
                float wave = dist * _RainbowWaveScale - time * _RainbowWaveSpeed;
                float irregularWave = sin(wave * 6.28318) * 0.5 + 0.5;
                irregularWave += sin(wave * 2.0 + distortionNoise2 * 3.0) * 0.3;
                irregularWave += simplexNoise2D(float2(wave, dist + time * 0.5) * 2.0) * 0.2;
                irregularWave = saturate(irregularWave);
                
                float shimmer = sin(time * 8.0 + screenUV.x * 20.0 + screenUV.y * 15.0) * 0.1 + 0.9;
                float3 rainbowColor = lerp(float3(1,1,1), colorShift, _RainbowSaturation);
                
                float intensity = irregularWave * falloff * shimmer * _RainbowIntensity;
                
                return rainbowColor * intensity;
            }

            v2f vert_shared(appdata IN, float ghostOffsetDir, out float4 outpos)
            {
                v2f OUT;

                float time = _Time.y + _RandomOffset;
                
                OUT.glitchActive = _ExternalGlitchActive * _GlitchIntensity;

                float4 worldPos = mul(unity_ObjectToWorld, IN.vertex);
                
                if (_VoxelSize > 0.001)
                {
                    float3 voxelized = floor(worldPos.xyz / _VoxelSize) * _VoxelSize;
                    worldPos.xyz = lerp(worldPos.xyz, voxelized, _VoxelBlend);
                }

                if (OUT.glitchActive > 0.01)
                {
                    float randX = hash(IN.vertex.x + time) - 0.5;
                    float randY = hash(IN.vertex.y + time + 1.0) - 0.5;
                    float randZ = hash(IN.vertex.z + time + 2.0) - 0.5;
                    float3 glitchOffset = float3(randX, randY, randZ) * _VertexGlitchAmount * OUT.glitchActive;
                    worldPos.xyz += glitchOffset;
                }

                OUT.worldPos = worldPos.xyz;
                
                outpos = mul(UNITY_MATRIX_VP, worldPos);

                // GHOST OFFSET
                if (abs(ghostOffsetDir) > 0.01 && OUT.glitchActive > 0.01)
                {
                    float4 centerClip = UnityObjectToClipPos(float4(0,0,0,1));
                    float2 centerScreen = centerClip.xy / centerClip.w;
                    float2 vertScreen = outpos.xy / outpos.w;
                    
                    float2 dir = normalize(vertScreen - centerScreen);
                    
                    float2 screenOffset = dir * _ColorGlitchAmount * OUT.glitchActive * 0.2 * ghostOffsetDir;
                    
                    outpos.xy += screenOffset * outpos.w;
                }

                OUT.screenPos = outpos;
                OUT.uv = IN.uv;
                OUT.normal = UnityObjectToWorldNormal(IN.normal);
                OUT.viewDir = normalize(WorldSpaceViewDir(IN.vertex));
                OUT.dissolveUV = TRANSFORM_TEX(IN.uv.xy, _DissolveTex);

                return OUT;
            }

            half4 frag_shared(v2f IN, float3 colorFilter, bool isGhost)
            {
                // Glitch inactif
                if (isGhost && IN.glitchActive < 0.01) discard;

                // Dissolve
                float dissolveVal = tex2D(_DissolveTex, IN.dissolveUV).r;
                float dissolveFade = smoothstep(_DissolveAmount - _DissolveEdge, _DissolveAmount + _DissolveEdge, dissolveVal);
                clip(dissolveFade - 0.001);

                half4 texcol = tex2D(_MainTex, IN.uv.xy);
                half4 texColor = texcol * _TextureTint;

                // Fresnel
                float rimDot = 1.0 - dot(normalize(IN.normal), IN.viewDir);
                float fresnel = rimDot * pow(rimDot, _RimPower);
                fresnel = smoothstep(_RimThreshold - 0.01, _RimThreshold + 0.01, fresnel);

                half4 baseColor = lerp(_MainTint, texColor, _TextureInfluence);
                half4 col = lerp(baseColor, _GlowTint, fresnel);

                // Rainbow effect
                float2 screenUV = IN.screenPos.xy / _ScreenParams.xy;
                float3 rainbowEffect = calculateRainbowEffect(screenUV, IN.worldPos, _Time.y + _RandomOffset);
                
                if (isGhost)
                {
                    // Ghost pass
                    float lum = Luminance(texcol.rgb);
                    
                    float viewDot = saturate(dot(normalize(IN.normal), IN.viewDir));
                    float ghostMask = 1.0 - pow(viewDot, 4.0);
                    
                    float intensity = lum * _ColorGlitchAmount * ghostMask * 2.0;
                    
                    col.rgb = colorFilter * intensity;
                    col.a = intensity;
                }

                // Main pass alpha adjustments
                if (!isGhost)
                {
                    col.rgb = col.rgb + rainbowEffect * (1.0 - Luminance(rainbowEffect) * 0.3);
                    
                    fixed lum = Luminance(texcol.rgb);
                    lum = pow(lum, _Contrast);
                    col.a *= lerp(1.0, sqrt(lum), _BrightnessToAlpha);
                    col.a = saturate(col.a + fresnel);
                }

                // Scanlines
                float time = _Time.y + _RandomOffset;
                float scanLinePattern = frac(IN.screenPos.y / _ScreenParams.y * _ScanLineCount + time * _ScanLineSpeed);
                float scanLineMask = step(1.0 - _ScanLineThickness, scanLinePattern);
                col.a *= scanLineMask;

                // Flicker
                float screenHeight = _ScreenParams.y;
                float flickerY = IN.screenPos.y / screenHeight;
                flickerY += time * _FlickerSpeed;
                col.a += _FlickerIntensity * pow(tex2D(_FlickerTex, float2(0, flickerY * _FlickerRepeat)), 1.2);

                // Dissolve Fade
                col.a *= dissolveFade;

                // Vertical Fade
                float screenY = IN.screenPos.y / _ScreenParams.y;
                float fadeEdge = 0.15;
                float fadeHeight = lerp(-fadeEdge, 1.0 + fadeEdge, _VerticalFade);
                float verticalFadeMask = 1.0 - smoothstep(fadeHeight - fadeEdge, fadeHeight, screenY);
                col.a *= verticalFadeMask;

                return saturate(col);
            }
        ENDCG

        // MAIN PASS
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite [_ZWriteMode]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            v2f vert(appdata IN, out float4 outpos : SV_POSITION)
            {
                return vert_shared(IN, 0.0, outpos);
            }

            half4 frag(v2f IN) : COLOR
            {
                return frag_shared(IN, float3(1,1,1), false);
            }
            ENDCG
        }

        // RED GHOST
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            v2f vert(appdata IN, out float4 outpos : SV_POSITION)
            {
                return vert_shared(IN, 1.0, outpos);
            }

            half4 frag(v2f IN) : COLOR
            {
                return frag_shared(IN, float3(1, 0, 0), true); // Red
            }
            ENDCG
        }

        // BLUE GHOST
        Pass
        {
            Blend SrcAlpha One
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            v2f vert(appdata IN, out float4 outpos : SV_POSITION)
            {
                return vert_shared(IN, -1.0, outpos);
            }

            half4 frag(v2f IN) : COLOR
            {
                return frag_shared(IN, float3(0, 0, 1), true); // Blue
            }
            ENDCG
        }

    }

}