using UnityEngine;

public class WeaponFovBoost : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera weaponCamera;

    [Header("FOV Settings - Sprint")]
    [SerializeField] private float sprintBaseFov = 60f;
    [SerializeField] private float sprintMaxFov = 75f;
    [SerializeField] private float sprintSpeedThreshold = 10f; // Max FOV'a ulaşmak için gereken hız
    [SerializeField] private float sprintFovTransitionSpeed = 8f;

    [Header("FOV Settings - Slide")]
    [SerializeField] private float slideBaseFov = 60f;
    [SerializeField] private float slideMaxFov = 85f;
    [SerializeField] private float slideSpeedThreshold = 15f; // Max FOV'a ulaşmak için gereken hız
    [SerializeField] private float slideFovTransitionSpeed = 10f;

    [Header("General Settings")]
    [SerializeField] private float normalFovTransitionSpeed = 6f;

    private PlayerController playerController;
    private float initialPlayerFov;
    private float initialWeaponFov;
    private float targetPlayerFov;
    private float targetWeaponFov;

    private void Start()
    {
        // PlayerController referansını al
        playerController = GetComponentInParent<PlayerController>();
        if (playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("[WeaponFovBoost] PlayerController not found!");
                enabled = false;
                return;
            }
        }

        // Başlangıç FOV değerlerini kaydet
        if (playerCamera != null)
        {
            initialPlayerFov = playerCamera.fieldOfView;
            targetPlayerFov = initialPlayerFov;
        }
        else
        {
            Debug.LogWarning("[WeaponFovBoost] Player Camera not assigned!");
        }

        if (weaponCamera != null)
        {
            initialWeaponFov = weaponCamera.fieldOfView;
            targetWeaponFov = initialWeaponFov;
        }
        else
        {
            Debug.LogWarning("[WeaponFovBoost] Weapon Camera not assigned!");
        }
    }

    private void Update()
    {
        if (playerController == null) return;

        // Mevcut hızı al
        float currentSpeed = playerController.GetVelocity();
        bool isSprinting = playerController.isSprinting;
        bool isSliding = playerController.isSliding;

        // Hedef FOV'u hesapla
        CalculateTargetFov(currentSpeed, isSprinting, isSliding);

        // FOV'u yumuşak şekilde hedefe götür
        ApplyFovTransition(isSprinting, isSliding);
    }

    /// <summary>
    /// Hız, sprint ve slide durumuna göre hedef FOV'u hesaplar
    /// </summary>
    private void CalculateTargetFov(float speed, bool isSprinting, bool isSliding)
    {
        if (isSliding)
        {
            // Slide: Hıza göre FOV hesapla (slideBaseFov → slideMaxFov)
            float speedRatio = Mathf.Clamp01(speed / slideSpeedThreshold);
            targetPlayerFov = Mathf.Lerp(slideBaseFov, slideMaxFov, speedRatio);
            targetWeaponFov = Mathf.Lerp(slideBaseFov, slideMaxFov, speedRatio);
        }
        else if (isSprinting)
        {
            // Sprint: Hıza göre FOV hesapla (sprintBaseFov → sprintMaxFov)
            float speedRatio = Mathf.Clamp01(speed / sprintSpeedThreshold);
            targetPlayerFov = Mathf.Lerp(sprintBaseFov, sprintMaxFov, speedRatio);
            targetWeaponFov = Mathf.Lerp(sprintBaseFov, sprintMaxFov, speedRatio);
        }
        else
        {
            // Normal: Başlangıç FOV'sine dön
            targetPlayerFov = initialPlayerFov;
            targetWeaponFov = initialWeaponFov;
        }
    }

    /// <summary>
    /// Mevcut FOV'u hedefe yumuşak şekilde götürür
    /// </summary>
    private void ApplyFovTransition(bool isSprinting, bool isSliding)
    {
        // Transition speed'i duruma göre belirle
        float transitionSpeed;
        if (isSliding)
        {
            transitionSpeed = slideFovTransitionSpeed;
        }
        else if (isSprinting)
        {
            transitionSpeed = sprintFovTransitionSpeed;
        }
        else
        {
            transitionSpeed = normalFovTransitionSpeed;
        }

        // Player Camera FOV
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = Mathf.Lerp(
                playerCamera.fieldOfView,
                targetPlayerFov,
                Time.deltaTime * transitionSpeed
            );
        }

        // Weapon Camera FOV
        if (weaponCamera != null)
        {
            weaponCamera.fieldOfView = Mathf.Lerp(
                weaponCamera.fieldOfView,
                targetWeaponFov,
                Time.deltaTime * transitionSpeed
            );
        }
    }

    /// <summary>
    /// FOV'u manuel olarak başlangıç değerine döndür
    /// </summary>
    public void ResetFov()
    {
        if (playerCamera != null)
        {
            playerCamera.fieldOfView = initialPlayerFov;
        }

        if (weaponCamera != null)
        {
            weaponCamera.fieldOfView = initialWeaponFov;
        }

        targetPlayerFov = initialPlayerFov;
        targetWeaponFov = initialWeaponFov;
    }

    /// <summary>
    /// Başlangıç FOV değerlerini güncelle
    /// </summary>
    public void SetInitialFov(float newPlayerFov, float newWeaponFov)
    {
        initialPlayerFov = newPlayerFov;
        initialWeaponFov = newWeaponFov;

        if (playerCamera != null)
        {
            playerCamera.fieldOfView = initialPlayerFov;
        }

        if (weaponCamera != null)
        {
            weaponCamera.fieldOfView = initialWeaponFov;
        }
    }

    /// <summary>
    /// Debug için mevcut durumu göster
    /// </summary>
    private void OnGUI()
    {
        if (!Debug.isDebugBuild) return;

        GUILayout.BeginArea(new Rect(10, 200, 300, 150));
        GUILayout.Label($"Speed: {playerController?.GetVelocity():F1}");
        GUILayout.Label($"Target Player FOV: {targetPlayerFov:F1}");
        GUILayout.Label($"Current Player FOV: {playerCamera?.fieldOfView:F1}");
        GUILayout.Label($"Sprint: {playerController?.isSprinting}");
        GUILayout.Label($"Slide: {playerController?.isSliding}");
        GUILayout.EndArea();
    }
}
