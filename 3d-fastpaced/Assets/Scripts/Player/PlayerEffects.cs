using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class PlayerEffects : MonoBehaviour
{
    public static PlayerEffects Instance { get; private set; } 
    

    [SerializeField] private Volume globalVolume;

    [Header("Lens Distortion Settings")]
    [SerializeField] private float startLensDistortionIntensity;
    [SerializeField] private float targetLensDistortionIntensity = -0.1f;
    [SerializeField] private float lensDistortionDuration = 0.5f;

    [Header("Hue Shift Settings")]
    [SerializeField] private float hueShiftAmplitude = 30f;      // Dalga genliði (max deðer)
    [SerializeField] private float hueShiftFrequency = 2f;       // Dalga frekansý (hýz)
    [SerializeField] private float hueShiftDuration = 2f;

    private LensDistortion lensDistortion;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        globalVolume.profile.TryGet<LensDistortion>(out lensDistortion);
        globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments);
    }

    private void Start()
    {
        lensDistortion.intensity.value = startLensDistortionIntensity;
        lensDistortion.active = true;

        colorAdjustments.hueShift.Override(0f);

        StartLensDistortionEffect();    
    }

    private void StartLensDistortionEffect()
    {
        if (lensDistortion != null)
        {
            StartCoroutine(LensDistortionCoroutine());
        }
        else
        {
            Debug.LogWarning("[PlayerEffects] Cannot start lens distortion - component is null!");
        }
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    StartLensDistortionEffect();
        //}
    }

    private IEnumerator LensDistortionCoroutine()
    {
        float elapsed = 0f;
        float startValue = lensDistortion.intensity.value;

        //  Coroutine ile smooth geçiþ
        while (elapsed < lensDistortionDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lensDistortionDuration);

            // Lerp ile smooth geçiþ
            lensDistortion.intensity.value = Mathf.Lerp(startValue, targetLensDistortionIntensity, t);

            yield return null; // Bir sonraki frame'e kadar bekle
        }

        // Kesin deðere ayarla
        lensDistortion.intensity.value = targetLensDistortionIntensity;
    }

    public void ColorHueShift(float duration)
    {
        StartCoroutine(ColorHueShiftCoroutine(duration));
    }

    private IEnumerator ColorHueShiftCoroutine(float duration)
    {
        float elapsed = 0f;
        
        float startValue = colorAdjustments.hueShift.value;
        float startValueLens = lensDistortion.intensity.value;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float sineWave = Mathf.Sin(elapsed * hueShiftFrequency * Mathf.PI * 2f) * hueShiftAmplitude;
            colorAdjustments.hueShift.value = sineWave;

            //float t = Mathf.Clamp01(elapsed / lensDistortionDuration);
            //lensDistortion.intensity.value = Mathf.Lerp(startValueLens, -0.9f, t);

            yield return null;
        }
        

        colorAdjustments.hueShift.value = 0f;
        lensDistortion.intensity.value = targetLensDistortionIntensity;
    }
}
