using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

// =====================================================================================
// WHITEOUT REVEAL RENDERER FEATURE (Unity 6 / URP RenderGraph)
// =====================================================================================
// Ne yapar:
//   - Sahnedeki her şeyi, oyuncuya (veya belirlediğin bir Transform'a) yakınlık
//     mesafesine göre kademeli olarak beyaza boyar (radius arttıkça efekt büyür).
//   - "Excluded Layers" içine koyduğun layer'lardaki objeler bu efektten TAMAMEN
//     muaf tutulur (VFX'in gibi), stencil buffer üzerinden maskelenir.
//
// Kurulum:
//   1. Bu feature'ı URP Renderer Data asset'ine ekle (Add Renderer Feature).
//   2. Inspector'dan Stencil Write Shader ve Whiteout Shader alanlarına
//      aşağıdaki shader dosyalarını ata.
//   3. Excluded Layers'a etkilenmesini istemediğin layer'ları (VFX gibi) seç.
//   4. Sahnende bir GameObject'e WhiteoutRevealController.cs'i ekle, Center alanına
//      oyuncuyu (veya efektin merkezini) sürükle, Radius'u zamanla artır.
// =====================================================================================
public class WhiteoutRevealFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class Settings
    {
        [Tooltip("Bu layer'lardaki objeler whiteout efektinden tamamen muaf tutulur. " +
                 "Birden fazla layer secebilirsin (orn: Weapon + Pickups + VFX). " +
                 "Not: Screen Space Overlay UI zaten otomatik muaf, buraya eklemene gerek yok.")]
        public LayerMask excludedLayers = 0;

        [Tooltip("Stencil buffer'a 'bu obje hariç tutulsun' damgasını vuran gizli shader")]
        public Shader stencilWriteShader;

        [Tooltip("Ekranı beyaza yaklaştıran fullscreen shader")]
        public Shader whiteoutShader;

        [Tooltip("ONEMLI: Bu deger, Edge Detection feature'inin RenderPassEvent'inden ONCE olmali " +
                 "(orn: bu AfterRenderingOpaques ise, Edge Detection BeforeRenderingPostProcessing veya sonrasi olsun). " +
                 "Aksi halde outline'lar beyazin altinda kalir, silinmis gibi gorunur.")]
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    public Settings settings = new Settings();

    private Material stencilMaterial;
    private Material whiteoutMaterial;

    private StencilExclusionPass stencilPass;
    private WhiteoutBlitPass whiteoutPass;

    public override void Create()
    {
        if (settings.stencilWriteShader != null)
            stencilMaterial = CoreUtils.CreateEngineMaterial(settings.stencilWriteShader);

        if (settings.whiteoutShader != null)
            whiteoutMaterial = CoreUtils.CreateEngineMaterial(settings.whiteoutShader);

        stencilPass = new StencilExclusionPass(stencilMaterial, settings.excludedLayers)
        {
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques
        };

        whiteoutPass = new WhiteoutBlitPass(whiteoutMaterial)
        {
            renderPassEvent = settings.renderPassEvent
        };
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Kamera oyun kamerası değilse (reflection probe, preview vb.) çalışma
        if (renderingData.cameraData.cameraType != CameraType.Game &&
            renderingData.cameraData.cameraType != CameraType.SceneView)
            return;

        // Weapon Camera gibi Overlay kameralar bu efekti hiç görmesin.
        // Aynı mantığı Edge Detection feature'ının AddRenderPasses'ine de eklemen gerekiyor,
        // yoksa outline'lar weapon camera'nın render ettiği objelerde de çıkmaya devam eder.
        if (renderingData.cameraData.renderType == CameraRenderType.Overlay)
            return;

        if (whiteoutMaterial == null || stencilMaterial == null)
        {
            Debug.LogWarning("WhiteoutRevealFeature: Shader referansları eksik, pass atlanıyor.");
            return;
        }

        // Sıra önemli: önce stencil'i işaretle, sonra fullscreen efekti uygula
        renderer.EnqueuePass(stencilPass);
        renderer.EnqueuePass(whiteoutPass);
    }

    protected override void Dispose(bool disposing)
    {
        CoreUtils.Destroy(stencilMaterial);
        CoreUtils.Destroy(whiteoutMaterial);
    }

    // =================================================================================
    // PASS 1: Hariç tutulan layer'ları stencil buffer'a işaretler.
    // Renk yazmaz (ColorMask 0), sadece "bu piksel muaf" damgasını basar.
    // =================================================================================
    class StencilExclusionPass : ScriptableRenderPass
    {
        private Material material;
        private LayerMask layerMask;
        private List<ShaderTagId> shaderTags;

        private class PassData
        {
            public RendererListHandle rendererList;
        }

        public StencilExclusionPass(Material mat, LayerMask mask)
        {
            material = mat;
            layerMask = mask;
            shaderTags = new List<ShaderTagId>
            {
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("SRPDefaultUnlit")
            };
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            // Excluded layer boşsa (hiçbir şey hariç tutulmuyorsa) pass'i atla
            if (layerMask.value == 0)
                return;

            UniversalRenderingData renderingData = frameData.Get<UniversalRenderingData>();
            UniversalCameraData cameraData = frameData.Get<UniversalCameraData>();
            UniversalLightData lightData = frameData.Get<UniversalLightData>();
            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            var filterSettings = new FilteringSettings(RenderQueueRange.all, layerMask);

            var drawSettings = RenderingUtils.CreateDrawingSettings(
                shaderTags, renderingData, cameraData, lightData, SortingCriteria.CommonOpaque);

            // Ne kullanılırsa kullanılsın objenin kendi shader'ını değil,
            // bizim stencil-yazan gizli materyalimizi zorla kullan.
            drawSettings.overrideMaterial = material;
            drawSettings.overrideMaterialPassIndex = 0;

            var listParams = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Whiteout Stencil Mask", out var passData))
            {
                passData.rendererList = renderGraph.CreateRendererList(listParams);
                builder.UseRendererList(passData.rendererList);

                // Sadece derinlik/stencil buffer'ı yazıyoruz, renk hedefine dokunmuyoruz
                builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.ReadWrite);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    ctx.cmd.DrawRendererList(data.rendererList);
                });
            }
        }
    }

    // =================================================================================
    // PASS 2: Tüm ekranı world-space mesafeye göre beyaza yaklaştırır.
    // Stencil'de "muaf" işareti olan pikselleri atlar (shader'daki Stencil bloğu ile).
    // =================================================================================
    class WhiteoutBlitPass : ScriptableRenderPass
    {
        private Material material;

        private class PassData
        {
            public TextureHandle source;
            public Material material;
        }

        public WhiteoutBlitPass(Material mat)
        {
            material = mat;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            if (material == null) return;

            UniversalResourceData resourceData = frameData.Get<UniversalResourceData>();

            TextureHandle cameraColor = resourceData.activeColorTexture;
            TextureHandle cameraDepth = resourceData.activeDepthTexture;

            var desc = renderGraph.GetTextureDesc(cameraColor);
            desc.name = "_WhiteoutRevealTarget";
            desc.clearBuffer = false;
            desc.depthBufferBits = 0;
            TextureHandle tempTarget = renderGraph.CreateTexture(desc);

            using (var builder = renderGraph.AddRasterRenderPass<PassData>("Whiteout Reveal", out var passData))
            {
                passData.source = cameraColor;
                passData.material = material;

                builder.UseTexture(cameraColor, AccessFlags.Read);
                builder.SetRenderAttachment(tempTarget, 0, AccessFlags.Write);
                // Depth'i stencil test + world-pos reconstruction için okuyoruz
                builder.SetRenderAttachmentDepth(cameraDepth, AccessFlags.Read);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
                {
                    data.material.SetTexture("_SourceTex", data.source);
                    Blitter.BlitTexture(ctx.cmd, data.source, new Vector4(1, 1, 0, 0), data.material, 0);
                });
            }

            // Sonucu kameranın ana renk hedefi olarak geri yaz
            resourceData.cameraColor = tempTarget;
        }
    }
}