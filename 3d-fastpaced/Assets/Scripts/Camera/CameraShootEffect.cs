using UnityEngine;

public class CameraShootEffect : MonoBehaviour
{
    [Header("Visual Impact Settings")]
    [SerializeField] private float impactAmount = 0.05f;        // Kameranýn ileri/geri hareket miktarý
    [SerializeField] private float kickRotation = 1.5f;         // Ani yukarý bakýþ
    [SerializeField] private float shakeIntensity = 0.02f;      // Sallanma þiddeti
    [SerializeField] private float angularFrequency = 25f;      // Hýz (yüksek = daha snappy)
    [SerializeField] private float dampingRatio = 0.6f;         // Az sallanma için düþük deðer
    
    // FOV punch effect
    [Header("FOV Kick")]
    [SerializeField] private bool useFovKick = true;
    [SerializeField] private float fovKickAmount = 5f;          // FOV geniþlemesi
    [SerializeField] private Camera targetCamera;
    private float originalFov;
    
    // Spring parametreleri
    private SpringUtils.tDampedSpringMotionParams springParams = new SpringUtils.tDampedSpringMotionParams();
    
    // Position spring (Z ekseni - punch)
    private float currentPosZ = 0f;
    private float velocityZ = 0f;
    private float targetPosZ = 0f;
    
    // Rotation spring (X ekseni - kick)
    private float currentRotX = 0f;
    private float velocityRotX = 0f;
    private float targetRotX = 0f;
    
    // Shake spring (Y ekseni - random shake)
    private float currentRotY = 0f;
    private float velocityRotY = 0f;
    private float targetRotY = 0f;
    
    // FOV spring
    private float currentFov = 0f;
    private float velocityFov = 0f;
    private float targetFov = 0f;
    
    private Vector3 originalLocalPosition;
    private Quaternion originalLocalRotation;

    private void Start()
    {
        originalLocalPosition = transform.localPosition;
        originalLocalRotation = transform.localRotation;
        
        if (targetCamera == null)
        {
            targetCamera = GetComponentInChildren<Camera>();
        }
        
        if (targetCamera != null)
        {
            originalFov = targetCamera.fieldOfView;
            currentFov = originalFov;
        }
        
        Debug.Log($"[CameraShootEffect] Visual impact initialized on {gameObject.name}");
    }

    private void Update()
    {
        // Spring hesapla
        SpringUtils.CalcDampedSpringMotionParams(
            ref springParams,
            Time.deltaTime,
            angularFrequency,
            dampingRatio
        );
        
        // Position punch (ileri/geri)
        SpringUtils.UpdateDampedSpringMotion(
            ref currentPosZ,
            ref velocityZ,
            targetPosZ,
            springParams
        );
        
        // Rotation kick (yukarý)
        SpringUtils.UpdateDampedSpringMotion(
            ref currentRotX,
            ref velocityRotX,
            targetRotX,
            springParams
        );
        
        // Shake (saða/sola)
        SpringUtils.UpdateDampedSpringMotion(
            ref currentRotY,
            ref velocityRotY,
            targetRotY,
            springParams
        );
        
        // FOV kick
        if (useFovKick && targetCamera != null)
        {
            SpringUtils.UpdateDampedSpringMotion(
                ref currentFov,
                ref velocityFov,
                targetFov,
                springParams
            );
            targetCamera.fieldOfView = originalFov + currentFov;
        }
        
        // Transform uygula
        Vector3 punchOffset = new Vector3(0, 0, currentPosZ);
        transform.localPosition = originalLocalPosition + punchOffset;
        
        Quaternion impactRotation = Quaternion.Euler(currentRotX, currentRotY, 0);
        transform.localRotation = originalLocalRotation * impactRotation;
    }

    /// <summary>
    /// Ateþ ederken görsel impact efekti
    /// </summary>
    public void ApplyShootImpact()
    {
        // Kýsa bir punch forward (sonra geri gelir)
        targetPosZ = impactAmount;
        
        // Ani yukarý kick
        targetRotX = -kickRotation;
        
        // Rastgele shake
        targetRotY = Random.Range(-shakeIntensity, shakeIntensity) * 10f;
        
        // FOV geniþlemesi (zoom out hissi)
        if (useFovKick)
        {
            targetFov = fovKickAmount;
        }
        
        // Hýzlýca sýfýrla (snappy his için)
        StartCoroutine(QuickReset());
    }
    
    /// <summary>
    /// Silaha özel impact deðerleri
    /// </summary>
    public void ApplyShootImpact(float posAmount, float rotAmount, float shakeAmount)
    {
        targetPosZ = posAmount;
        targetRotX = -rotAmount;
        targetRotY = Random.Range(-shakeAmount, shakeAmount) * 10f;
        
        if (useFovKick)
        {
            targetFov = fovKickAmount * (rotAmount / kickRotation);
        }
        
        StartCoroutine(QuickReset());
    }
    
    private System.Collections.IEnumerator QuickReset()
    {
        // Kýsa bir süre sonra hedefleri sýfýrla (spring doðal olarak geri dönsün)
        yield return new WaitForSeconds(0.05f);
        
        targetPosZ = 0f;
        targetRotX = 0f;
        targetRotY = 0f;
        targetFov = 0f;
    }
}
