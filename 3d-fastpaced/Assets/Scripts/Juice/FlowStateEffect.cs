using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlowStateEffect : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private LevelDataSO currentLevelData;
    [SerializeField] private Color flowColor = Color.red;

    [Header("Global Volume")]
    [SerializeField] private Volume globalVolume;
    [SerializeField] private float lensDistortionTarget = -1f;
    [SerializeField] private float lensDistortionInDuration = 0.2f;
    [SerializeField] private float lensDistortionOutDuration = 0.3f;

    [Header("Fullscreen Radius Shader")]
    [SerializeField] private Material fullscreenMaterial;
    [SerializeField] private float targetRadius = 1f;
    [SerializeField] private float radiusTransitionDuration = 0.4f;

    private static readonly int RadiusID = Shader.PropertyToID("_Radius");

    private LensDistortion lensDistortion;
    private bool isFlowActive = false;

    private Coroutine lensPulseRoutine;
    private Coroutine radiusRoutine;

    private void Awake()
    {
        if (globalVolume != null && globalVolume.profile.TryGet(out lensDistortion))
        {
            // Override'ýn aktif olduðundan emin ol, yoksa .value deðiþikliði görünmez
            lensDistortion.intensity.overrideState = true;
        }
    }

    private void Update()
    {
        if (InputManager.Instance.crouchAction.WasPressedThisFrame())
        {
            ToggleFlowState();
        }
    }

    private void ToggleFlowState()
    {
        isFlowActive = !isFlowActive;

        if (isFlowActive)
        {
            EnterFlowState();
        }
        else
        {
            ExitFlowState();
        }
    }

    private void EnterFlowState()
    {
        ParticlePaintManager.Instance.ApplyOverride(flowColor);

        if (radiusRoutine != null) StopCoroutine(radiusRoutine);
        radiusRoutine = StartCoroutine(LerpRadius(targetRadius, radiusTransitionDuration));

        // Lens distortion, exit'i beklemeden kendi baþýna gidip geri dönüyor
        if (lensPulseRoutine != null) StopCoroutine(lensPulseRoutine);
        lensPulseRoutine = StartCoroutine(PulseLensDistortion());
    }

    private void ExitFlowState()
    {
        ParticlePaintManager.Instance.RevertToDefault();

        if (radiusRoutine != null) StopCoroutine(radiusRoutine);
        radiusRoutine = StartCoroutine(LerpRadius(-1f, radiusTransitionDuration));

        // Not: Lens distortion'a burada dokunmuyoruz, çünkü o zaten
        // enter anýnda kendi pulse'ýný tamamlayýp eski haline dönmüþ oluyor.
    }

    private IEnumerator PulseLensDistortion()
    {
        if (lensDistortion == null) yield break;

        float originalValue = lensDistortion.intensity.value;

        yield return LerpLensValue(originalValue, lensDistortionTarget, lensDistortionInDuration);
        yield return LerpLensValue(lensDistortionTarget, originalValue, lensDistortionOutDuration);
    }

    private IEnumerator LerpLensValue(float from, float to, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            lensDistortion.intensity.value = Mathf.Lerp(from, to, normalized);
            yield return null;
        }

        lensDistortion.intensity.value = to;
    }

    private IEnumerator LerpRadius(float target, float duration)
    {
        if (fullscreenMaterial == null) yield break;

        float startValue = fullscreenMaterial.GetFloat(RadiusID);
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = Mathf.Clamp01(t / duration);
            fullscreenMaterial.SetFloat(RadiusID, Mathf.Lerp(startValue, target, normalized));
            yield return null;
        }

        fullscreenMaterial.SetFloat(RadiusID, target);
    }
}