// Hariç tutulan layer'daki objeleri stencil buffer'a "muaf" olarak işaretler.
// Renk hedefine dokunmaz (ColorMask 0) - sadece stencil damgası basar.
Shader "Hidden/WhiteoutReveal/StencilWriteOnly"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "StencilWrite"

            ColorMask 0
            ZWrite Off
            // Zaten yazılmış derinlikle eşleşen (yani gerçekten görünen) pikselleri işaretle
            ZTest Equal

            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };

            Varyings Vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 Frag(Varyings IN) : SV_Target
            {
                return 0;
            }
            ENDHLSL
        }
    }
}
