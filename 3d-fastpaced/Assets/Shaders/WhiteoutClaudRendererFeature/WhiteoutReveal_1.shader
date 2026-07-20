// Kamera renk buffer'ini alir, her pikselin world-space pozisyonunu derinlikten
// yeniden hesaplar, _WhiteoutCenter'a olan mesafeye gore beyaza yaklastirir.
// Stencil'de "muaf" (Ref 1) isaretli pikselleri atlar - o pikseller orijinal renginde kalir.
Shader "Hidden/WhiteoutReveal/Fullscreen"
{
    Properties
    {
        _SourceTex ("Source", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "WhiteoutReveal"

            ZWrite Off
            ZTest Always
            Cull Off

            // Stencil'de 1 isaretli (muaf) pikselleri atla - sadece 0 olanlara uygula
            Stencil
            {
                Ref 1
                Comp NotEqual
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            TEXTURE2D(_SourceTex);
            SAMPLER(sampler_SourceTex);

            // Bunlar script tarafindan Shader.SetGlobalX ile her frame gonderiliyor
            float3 _WhiteoutCenter;
            float _WhiteoutRadius;
            float _WhiteoutSoftness;
            float3 _WhiteoutColor;

            half4 Frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.texcoord;
                half4 col = SAMPLE_TEXTURE2D(_SourceTex, sampler_SourceTex, uv);

                // Derinlikten world-space pozisyonu geri hesapla
                float rawDepth = SampleSceneDepth(uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, rawDepth, UNITY_MATRIX_I_VP);

                float dist = distance(worldPos, _WhiteoutCenter);

                // ONEMLI: HLSL'in smoothstep(a,b,x) fonksiyonu a<b siralamasi bekler,
                // aksi halde platforma gore degisen tanimsiz sonuc verir.
                // Bu yuzden once dogru sirada (kucukten buyuge) hesaplayip sonra tersine ceviriyoruz:
                //   dist <= (radius-softness) -> icerde  -> raw = 0 -> mask = 1 (beyaz)
                //   dist >= radius            -> disarda -> raw = 1 -> mask = 0 (dokunma)
                float loEdge = max(_WhiteoutRadius - _WhiteoutSoftness, 0.0);
                float hiEdge = _WhiteoutRadius;
                float raw = smoothstep(loEdge, hiEdge, dist);
                float mask = 1.0 - raw;

                col.rgb = lerp(col.rgb, _WhiteoutColor, mask);
                return col;
            }
            ENDHLSL
        }
    }
}
