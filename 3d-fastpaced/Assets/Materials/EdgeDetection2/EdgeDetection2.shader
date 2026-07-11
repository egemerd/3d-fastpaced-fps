Shader "Hidden/Edge Detection 2"
{
    Properties
    {
        _OutlineThickness ("Outline Thickness", Float) = 1
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)

        [Header(Moebius Wiggle)]
        _WiggleAmount ("Wiggle Amount (texel)", Float) = 2.0
        _WiggleScale ("Wiggle Noise Scale", Float) = 0.02

        [Header(Moebius Thickness Variation)]
        _ThicknessVariation ("Thickness Variation", Range(0, 1)) = 0.6
        _ThicknessNoiseScale ("Thickness Noise Scale (buyuk = genis bolgeler)", Float) = 4.0
        _ThicknessThreshold ("Thickness Threshold", Range(0, 1)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType"="Opaque"
        }

        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            Name "EDGE DETECTION OUTLINE"
            
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl" // needed to sample scene depth
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareNormalsTexture.hlsl" // needed to sample scene normals
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl" // needed to sample scene color/luminance

            float _OutlineThickness;
            float4 _OutlineColor;

            float _WiggleAmount;
            float _WiggleScale;
            float _ThicknessVariation;
            float _ThicknessNoiseScale;
            float _ThicknessThreshold;

            #pragma vertex Vert // vertex shader is provided by the Blit.hlsl include
            #pragma fragment frag

            // Edge detection kernel that works by taking the sum of the squares of the differences between diagonally adjacent pixels (Roberts Cross).
            float RobertsCross(float3 samples[4])
            {
                const float3 difference_1 = samples[1] - samples[2];
                const float3 difference_2 = samples[0] - samples[3];
                return sqrt(dot(difference_1, difference_1) + dot(difference_2, difference_2));
            }

            // The same kernel logic as above, but for a single-value instead of a vector3.
            float RobertsCross(float samples[4])
            {
                const float difference_1 = samples[1] - samples[2];
                const float difference_2 = samples[0] - samples[3];
                return sqrt(difference_1 * difference_1 + difference_2 * difference_2);
            }
            
            // Helper function to sample scene normals remapped from [-1, 1] range to [0, 1].
            float3 SampleSceneNormalsRemapped(float2 uv)
            {
                return SampleSceneNormals(uv) * 0.5 + 0.5;
            }

            // Helper function to sample scene luminance.
            float SampleSceneLuminance(float2 uv)
            {
                float3 color = SampleSceneColor(uv);
                return color.r * 0.3 + color.g * 0.59 + color.b * 0.11;
            }

            // --- Moebius stili icin: el yapimi basit bir value noise fonksiyonu ---
            // Shader Graph'taki "Simple Noise" node'unun raw HLSL karsiligi gibi dusun.
            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);
                return frac(p.x * p.y);
            }

            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                float a = Hash21(i);
                float b = Hash21(i + float2(1.0, 0.0));
                float c = Hash21(i + float2(0.0, 1.0));
                float d = Hash21(i + float2(1.0, 1.0));

                // Smoothstep ile yumusak gecis (blocky degil, organik gorunum icin)
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            half4 frag(Varyings IN) : SV_TARGET
            {
                // Screen-space coordinates which we will use to sample.
                float2 uv = IN.texcoord;
                float2 texel_size = float2(1.0 / _ScreenParams.x, 1.0 / _ScreenParams.y);

                // --- MOEBIUS WIGGLE ---
                // Orijinal uv'yi dusuk frekansli noise ile hafifce kaydiriyoruz.
                // Bu, kenar tespitinin "nereye baktigini" titretir - duz cizgi yerine
                // elle cizilmis, titrek bir cizgi hissi verir.
                float2 wiggleNoise = float2(
                    ValueNoise(uv / _WiggleScale),
                    ValueNoise(uv / _WiggleScale + 17.0)
                ) - 0.5; // -0.5..0.5 araligina cek, her iki yone de kayabilsin
                uv += wiggleNoise * texel_size * _WiggleAmount;

                // --- MOEBIUS THICKNESS VARIATION ---
                // Kalinligi da ayri, daha yavas degisen bir noise ile belirli bolgelerde
                // kalin, belirli bolgelerde ince olacak sekilde moduluyoruz.
                float thicknessNoise = ValueNoise(uv / _ThicknessNoiseScale);
                float thicknessFactor = smoothstep(_ThicknessThreshold - 0.15, _ThicknessThreshold + 0.15, thicknessNoise);
                float variedThickness = lerp(
                    _OutlineThickness * (1.0 - _ThicknessVariation),
                    _OutlineThickness * (1.0 + _ThicknessVariation),
                    thicknessFactor
                );
                variedThickness = max(variedThickness, 0.0);

                // Generate 4 diagonally placed samples.
                const float half_width_f = floor(variedThickness * 0.5);
                const float half_width_c = ceil(variedThickness * 0.5);

                float2 uvs[4];
                uvs[0] = uv + texel_size * float2(half_width_f, half_width_c) * float2(-1, 1);  // top left
                uvs[1] = uv + texel_size * float2(half_width_c, half_width_c) * float2(1, 1);   // top right
                uvs[2] = uv + texel_size * float2(half_width_f, half_width_f) * float2(-1, -1); // bottom left
                uvs[3] = uv + texel_size * float2(half_width_c, half_width_f) * float2(1, -1);  // bottom right
                
                float3 normal_samples[4];
                float depth_samples[4], luminance_samples[4];
                
                for (int i = 0; i < 4; i++) {
                    depth_samples[i] = SampleSceneDepth(uvs[i]);
                    normal_samples[i] = SampleSceneNormalsRemapped(uvs[i]);
                    luminance_samples[i] = SampleSceneLuminance(uvs[i]);
                }
                
                // Apply edge detection kernel on the samples to compute edges.
                float edge_depth = RobertsCross(depth_samples);
                float edge_normal = RobertsCross(normal_samples);
                float edge_luminance = RobertsCross(luminance_samples);
                
                // Threshold the edges (discontinuity must be above certain threshold to be counted as an edge). The sensitivities are hardcoded here.
                float depth_threshold = 1 / 200.0f;
                edge_depth = edge_depth > depth_threshold ? 1 : 0;
                
                float normal_threshold = 1 / 4.0f;
                edge_normal = edge_normal > normal_threshold ? 1 : 0;
                
                float luminance_threshold = 1 / 0.5f;
                edge_luminance = edge_luminance > luminance_threshold ? 1 : 0;
                
                // Combine the edges from depth/normals/luminance using the max operator.
                float edge = max(edge_depth, max(edge_normal, edge_luminance));
                
                // Color the edge with a custom color.
                return edge * _OutlineColor;
            }
            ENDHLSL
        }
    }
}