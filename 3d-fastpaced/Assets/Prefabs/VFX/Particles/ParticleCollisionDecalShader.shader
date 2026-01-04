Shader "Custom/OrganicSplatDecal"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Smoothness ("Edge Smoothness", Range(0, 0.5)) = 0.1
        _Seed ("Random Seed", Range(0, 100)) = 0
        _Complexity ("Shape Complexity", Range(0.1, 5)) = 2.0
        _Distortion ("Edge Distortion", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };
            
            float4 _Color;
            float _Smoothness;
            float _Seed;
            float _Complexity;
            float _Distortion;
            
            // Basit noise fonksiyonu
            float hash(float2 p)
            {
                p = frac(p * float2(443.897, 441.423));
                p += dot(p, p + 19.19);
                return frac(p.x * p.y);
            }
            
            // Smooth noise
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                f = f * f * (3.0 - 2.0 * f); // Smoothstep
                
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                
                return lerp(lerp(a, b, f.x), lerp(c, d, f.x), f.y);
            }
            
            // Fractal Brownian Motion - daha organik görünüm için
            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;
                float frequency = 1.0;
                
                for(int i = 0; i < 4; i++)
                {
                    value += amplitude * noise(p * frequency);
                    frequency *= 2.0;
                    amplitude *= 0.5;
                }
                
                return value;
            }
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // UV'yi merkeze taþý
                float2 centered = i.uv - 0.5;
                
                // Seed ile offset ekle
                float2 seedOffset = float2(_Seed * 0.123, _Seed * 0.456);
                
                // Merkeze olan uzaklýk
                float dist = length(centered);
                
                // Noise ile kenarlarý bozma
                float2 noiseCoord = (centered + seedOffset) * _Complexity;
                float noiseValue = fbm(noiseCoord);
                
                // Dairesel þekle noise ekle
                float distortedRadius = 0.5 + (noiseValue - 0.5) * _Distortion;
                
                // Organik þekil oluþtur
                float shape = 1.0 - smoothstep(distortedRadius - _Smoothness, distortedRadius, dist);
                
                // Ýç kýsýmlara da hafif varyasyon ekle
                float innerNoise = fbm((centered + seedOffset) * _Complexity * 2.0);
                shape *= lerp(0.7, 1.0, innerNoise);
                
                fixed4 col = _Color;
                col.a *= shape;
                
                return col;
            }
            ENDCG
        }
    }
}