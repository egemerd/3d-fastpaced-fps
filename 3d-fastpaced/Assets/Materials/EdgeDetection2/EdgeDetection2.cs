using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class EdgeDetection2 : ScriptableRendererFeature
{
    private class EdgeDetectionPass : ScriptableRenderPass
    {
        private Material material;

        private static readonly int OutlineThicknessProperty = Shader.PropertyToID("_OutlineThickness");
        private static readonly int OutlineColorProperty = Shader.PropertyToID("_OutlineColor");
        private static readonly int WiggleAmountProperty = Shader.PropertyToID("_WiggleAmount");
        private static readonly int WiggleScaleProperty = Shader.PropertyToID("_WiggleScale");
        private static readonly int ThicknessVariationProperty = Shader.PropertyToID("_ThicknessVariation");
        private static readonly int ThicknessNoiseScaleProperty = Shader.PropertyToID("_ThicknessNoiseScale");
        private static readonly int ThicknessThresholdProperty = Shader.PropertyToID("_ThicknessThreshold");

        public EdgeDetectionPass()
        {
            profilingSampler = new ProfilingSampler(nameof(EdgeDetectionPass));
        }

        public void Setup(ref EdgeDetectionSettings settings, ref Material edgeDetectionMaterial)
        {
            material = edgeDetectionMaterial;
            renderPassEvent = settings.renderPassEvent;

            material.SetFloat(OutlineThicknessProperty, settings.outlineThickness);
            material.SetColor(OutlineColorProperty, settings.outlineColor);
            material.SetFloat(WiggleAmountProperty, settings.wiggleAmount);
            material.SetFloat(WiggleScaleProperty, settings.wiggleScale);
            material.SetFloat(ThicknessVariationProperty, settings.thicknessVariation);
            material.SetFloat(ThicknessNoiseScaleProperty, settings.thicknessNoiseScale);
            material.SetFloat(ThicknessThresholdProperty, settings.thicknessThreshold);
        }

        private class PassData
        {
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var resourceData = frameData.Get<UniversalResourceData>();

            using var builder = renderGraph.AddRasterRenderPass<PassData>("Edge Detection", out _);

            builder.SetRenderAttachment(resourceData.activeColorTexture, 0);
            builder.UseAllGlobalTextures(true);
            builder.AllowPassCulling(false);
            builder.SetRenderFunc((PassData _, RasterGraphContext context) => { Blitter.BlitTexture(context.cmd, Vector2.one, material, 0); });
        }
    }

    [Serializable]
    public class EdgeDetectionSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        [Range(0, 15)] public int outlineThickness = 3;
        public Color outlineColor = Color.black;

        [Header("Moebius Wiggle")]
        [Tooltip("Cizginin ne kadar titreyecegi (texel biriminde). 0 = duz cizgi, buyudukce daha titrek.")]
        public float wiggleAmount = 2.0f;
        [Tooltip("Titreme noise'unun buyuklugu. Kucuk = sik/hizli titreme, buyuk = yumusak/yavas dalgalanma.")]
        public float wiggleScale = 0.02f;

        [Header("Moebius Thickness Variation")]
        [Range(0, 1)]
        [Tooltip("Kalinlik farkinin siddeti. 0 = her yerde ayni kalinlik, 1 = maksimum kalin/ince fark.")]
        public float thicknessVariation = 0.6f;
        [Tooltip("Kalin/ince bolgelerin buyuklugu. Buyuk deger = genis, yavas degisen bolgeler.")]
        public float thicknessNoiseScale = 4.0f;
        [Range(0, 1)]
        [Tooltip("Noise'un hangi degerin ustunde 'kalin' bolge sayilacagini belirler.")]
        public float thicknessThreshold = 0.5f;
    }

    [SerializeField] private EdgeDetectionSettings settings;
    private Material edgeDetectionMaterial;
    private EdgeDetectionPass edgeDetectionPass;

    /// <summary>
    /// Called
    /// - When the Scriptable Renderer Feature loads the first time.
    /// - When you enable or disable the Scriptable Renderer Feature.
    /// - When you change a property in the Inspector window of the Renderer Feature.
    /// </summary>
    public override void Create()
    {
        edgeDetectionPass ??= new EdgeDetectionPass();
    }

    /// <summary>
    /// Called
    /// - Every frame, once for each camera.
    /// </summary>
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Don't render for some views.
        if (renderingData.cameraData.cameraType == CameraType.Preview
            || renderingData.cameraData.cameraType == CameraType.Reflection
            || UniversalRenderer.IsOffscreenDepthTexture(ref renderingData.cameraData))
            return;

        if (edgeDetectionMaterial == null)
        {
            edgeDetectionMaterial = CoreUtils.CreateEngineMaterial(Shader.Find("Hidden/Edge Detection 2"));
            if (edgeDetectionMaterial == null)
            {
                Debug.LogWarning("Not all required materials could be created. Edge Detection will not render.");
                return;
            }
        }

        edgeDetectionPass.ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal | ScriptableRenderPassInput.Color);
        edgeDetectionPass.requiresIntermediateTexture = true;
        edgeDetectionPass.Setup(ref settings, ref edgeDetectionMaterial);

        renderer.EnqueuePass(edgeDetectionPass);
    }

    /// <summary>
    /// Clean up resources allocated to the Scriptable Renderer Feature such as materials.
    /// </summary>
    override protected void Dispose(bool disposing)
    {
        edgeDetectionPass = null;
        CoreUtils.Destroy(edgeDetectionMaterial);
    }
}