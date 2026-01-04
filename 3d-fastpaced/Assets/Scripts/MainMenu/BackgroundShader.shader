Shader "Custom/MetaballBackground"
{
    Properties
    {
        _BackgroundColor ("Background Color", Color) = (0.05, 0.05, 0.1, 1)
        _BlobColor ("Blob Color", Color) = (1, 1, 1, 1)
        _GlowColor ("Glow Color", Color) = (0.8, 0.9, 1, 1)
        
        _BlobCount ("Blob Count", Float) = 8
        _BlobSize ("Blob Size", Range(0.1, 2)) = 0.5
        _Threshold ("Metaball Threshold", Range(0, 1)) = 0.5
        
        _Speed ("Movement Speed", Range(0, 2)) = 0.3
        _MouseInfluence ("Mouse Influence", Range(0, 5)) = 2.0
        _MouseRadius ("Mouse Radius", Range(0, 2)) = 0.8
        
        _GlowStrength ("Glow Strength", Range(0, 5)) = 2.0
        _EdgeSoftness ("Edge Softness", Range(0, 0.5)) = 0.1
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100
        
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
            
            float4 _BackgroundColor;
            float4 _BlobColor;
            float4 _GlowColor;
            float _BlobCount;
            float _BlobSize;
            float _Threshold;
            float _Speed;
            float _MouseInfluence;
            float _MouseRadius;
            float _GlowStrength;
            float _EdgeSoftness;
            float4 _MousePosition;
            
            // Hash fonksiyonu - random sayýlar için
            float hash(float n)
            {
                return frac(sin(n) * 43758.5453123);
            }
            
            // 2D hash
            float2 hash2(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)), dot(p, float2(269.5, 183.3)));
                return frac(sin(p) * 43758.5453);
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
                float2 uv = i.uv;
                float time = _Time.y * _Speed;
                
                // Metaball deðerini hesapla
                float metaball = 0.0;
                
                // Mouse pozisyonu
                float2 mouseUV = _MousePosition.xy;
                float distToMouse = distance(uv, mouseUV);
                
                // Birden fazla blob oluþtur
                for(int j = 0; j < _BlobCount; j++)
                {
                    float fi = float(j);
                    
                    // Her blob için unique hareket paterni
                    float2 seed = float2(fi * 0.1, fi * 0.2);
                    float2 offset = hash2(seed);
                    
                    // Blob pozisyonu - organik hareket
                    float2 blobPos = float2(
                        0.5 + sin(time * (0.5 + offset.x * 0.5) + fi * 2.0) * 0.3,
                        0.5 + cos(time * (0.4 + offset.y * 0.5) + fi * 1.5) * 0.3
                    );
                    
                    // Mouse'a yakýn bloblara etki
                    float distMouseToBlob = distance(mouseUV, blobPos);
                    if(distMouseToBlob < _MouseRadius)
                    {
                        float mouseEffect = (1.0 - distMouseToBlob / _MouseRadius) * _MouseInfluence;
                        float2 dirFromMouse = normalize(blobPos - mouseUV);
                        blobPos += dirFromMouse * mouseEffect * 0.1;
                    }
                    
                    // Blob büyüklüðü - zamanla deðiþen
                    float blobSize = _BlobSize * (0.8 + sin(time * 2.0 + fi * 3.0) * 0.2);
                    
                    // Blob'a olan uzaklýk
                    float dist = distance(uv, blobPos);
                    
                    // Metaball formülü - yakýn bloblarda deðer artar
                    float influence = blobSize / (dist * dist + 0.001);
                    metaball += influence;
                }
                
                // Mouse'un kendisi de bir blob gibi davransýn
                float mouseDist = distToMouse;
                float mouseBlob = (_BlobSize * 1.5) / (mouseDist * mouseDist + 0.001);
                metaball += mouseBlob * 2.0;
                
                // Threshold ile blob sýnýrlarýný belirle
                float blobMask = smoothstep(_Threshold - _EdgeSoftness, _Threshold + _EdgeSoftness, metaball);
                
                // Glow efekti - blob kenarlarýnda
                float glowMask = smoothstep(_Threshold - 0.2, _Threshold, metaball);
                glowMask *= (1.0 - blobMask);
                glowMask *= _GlowStrength;
                
                // Ýç kýsým parlaklýðý
                float innerGlow = smoothstep(_Threshold, _Threshold + 0.5, metaball);
                
                // Renkleri karýþtýr
                float4 finalColor = _BackgroundColor;
                
                // Blob rengi
                finalColor = lerp(finalColor, _BlobColor, blobMask);
                
                // Ýç parlaklýk
                finalColor.rgb += innerGlow * 0.3;
                
                // Kenar glow
                finalColor.rgb += _GlowColor.rgb * glowMask;
                
                // Mouse yakýnýnda ekstra parlaklýk
                float mouseGlow = smoothstep(_MouseRadius * 1.5, 0.0, distToMouse);
                finalColor.rgb += mouseGlow * 0.2;
                
                return finalColor;
            }
            ENDCG
        }
    }
}