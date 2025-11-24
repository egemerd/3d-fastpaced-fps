using System.Collections;
using UnityEngine;

public class LedgeClimbing : MonoBehaviour
{
    [Header("Detection Settings")]
    [SerializeField] private float ledgeClimbDistance = 1.5f;
    [SerializeField] private float heightMultiplier = 1.5f;
    [SerializeField] private LayerMask ledgeLayer;
    [SerializeField] private Transform minLedgePoint;
    [SerializeField] private Transform orientation;
    [SerializeField] private float secondRaycastForwardDistance;

    [Header("Climbing Settings")]
    [SerializeField] private float lerpDuration = 0.5f;
    [SerializeField] private float targetHeightOffset = 0.1f;

    [Header("Exit Boost Settings")] //  YENÝ: Çýkýþ momentum ayarlarý
    [SerializeField] private float exitForwardBoost = 6f; // Ýleriye doðru hýz
    [SerializeField] private float exitUpwardBoost = 3f; // Yukarý doðru hýz
    [SerializeField] private bool maintainHorizontalMomentum = true; // Yatay momentumu koru

    // Component References
    private InputManager input;
    private CharacterController characterController;
    private PlayerController playerController;

    // State
    public bool isLedgeClimbing { get; private set; }
    public bool isLedgeDetected { get; private set; }
    private Vector3 targetPosition;
    private Vector3 entryVelocity;

    private void Start()
    {
        input = InputManager.Instance;
        characterController = GetComponent<CharacterController>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        DetectLedge();
    }

    public bool CheckLedgeDetection()
    {
        if (Physics.Raycast(minLedgePoint.position, orientation.forward, out var firstHit, ledgeClimbDistance, ledgeLayer))
        {
            Vector3 secondStart = firstHit.point + secondRaycastForwardDistance * orientation.forward + Vector3.up * heightMultiplier * characterController.height;

            if (Physics.Raycast(secondStart, Vector3.down, out var secondHit, characterController.height, ledgeLayer))
            {
                float characterHalfHeight = characterController.height / 2f;
                targetPosition = secondHit.point +
                                Vector3.up * (characterHalfHeight + targetHeightOffset) +
                                orientation.forward * 0.3f;
                isLedgeDetected = true;
                return true;
            }
        }

        isLedgeDetected = false;
        return false;
    }

    private void DetectLedge()
    {
        CheckLedgeDetection();
    }

    public void StartLedgeClimb(PlayerController player)
    {
        if (!isLedgeDetected) return;

        Debug.Log("Starting Ledge Climb");
        isLedgeClimbing = true;

        if (maintainHorizontalMomentum)
        {
            entryVelocity = new Vector3(player.GetComponent<CharacterController>().velocity.x,
                                       0,
                                       player.GetComponent<CharacterController>().velocity.z);
        }


        // CharacterController'ý kapat
        characterController.enabled = false;

        

        // Coroutine baþlat
        StartCoroutine(LerpToLedge(player));
    }

    private IEnumerator LerpToLedge(PlayerController player)
    {
        Debug.Log("Lerping to Ledge Top");

        Vector3 startPos = transform.position;
        float elapsed = 0f;

        while (elapsed < lerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / lerpDuration);

            // Smooth curve (ease-out)
            float smoothT = 1f - Mathf.Pow(1f - t, 3f);

            transform.position = Vector3.Lerp(startPos, targetPosition, smoothT);

            yield return null;
        }

        // Final position
        transform.position = targetPosition;

        // Cleanup
        StopLedgeClimb(player);
    }

    public void StopLedgeClimb(PlayerController player)
    {
        Debug.Log("Ledge Climb Complete");
        isLedgeClimbing = false;

        // CharacterController'ý tekrar aç
        characterController.enabled = true;

        if (player != null)
        {
            ApplyExitBoost(player);
        }
    }

    private void ApplyExitBoost(PlayerController player)
    {
        Vector3 boostVelocity = Vector3.zero;

        // Ýleriye doðru boost (yön = orientation.forward)
        boostVelocity += orientation.forward * exitForwardBoost;

        // Yukarý doðru boost (küçük hop)
        boostVelocity += Vector3.up * exitUpwardBoost;

        // Eðer momentum korunacaksa, giriþ hýzýný ekle
        if (maintainHorizontalMomentum)
        {
            boostVelocity.x += entryVelocity.x * 0.5f; // %50 oranýnda ekle
            boostVelocity.z += entryVelocity.z * 0.5f;
        }

        // PlayerController'a velocity'yi aktar
        // DÝKKAT: PlayerController'da velocity public olmalý veya SetVelocity metodu ekle
        StartCoroutine(ApplyBoostNextFrame(boostVelocity));
    }

    private IEnumerator ApplyBoostNextFrame(Vector3 boostVelocity)
    {
        yield return null; // 1 frame bekle

        if (characterController.enabled)
        {
            // CharacterController.Move kullanarak momentum ver
            characterController.Move(boostVelocity * Time.deltaTime);

            Debug.Log($"Exit Boost Applied: {boostVelocity}");
        }
    }

    public Vector3 GetTargetPosition()
    {
        return targetPosition;
    }

    private void OnDrawGizmos()
    {
        if (minLedgePoint == null || orientation == null) return;

        Vector3 firstStart = minLedgePoint.position;
        Vector3 firstDirection = orientation.forward;
        float firstLength = ledgeClimbDistance;

        // First raycast
        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(firstStart, firstDirection * firstLength);
        Gizmos.DrawWireSphere(firstStart, 0.1f);

        if (Application.isPlaying && characterController != null)
        {
            if (Physics.Raycast(firstStart, firstDirection, out var firstHit, firstLength, ledgeLayer))
            {
                // First hit
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(firstHit.point, 0.15f);
                Gizmos.DrawLine(firstStart, firstHit.point);

                // Second raycast
                Vector3 secondStart = firstHit.point + secondRaycastForwardDistance * orientation.forward + Vector3.up * heightMultiplier *characterController.height;
                float secondLength = characterController.height;

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(secondStart, 0.1f);
                Gizmos.DrawRay(secondStart, Vector3.down * secondLength);

                if (Physics.Raycast(secondStart, Vector3.down, out var secondHit, secondLength, ledgeLayer))
                {
                    // Second hit (ledge üstü)
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(secondHit.point, 0.15f);
                    Gizmos.DrawLine(secondStart, secondHit.point);

                    // Target position
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawWireCube(secondHit.point, Vector3.one * 0.3f);
                    Gizmos.DrawLine(transform.position, secondHit.point);
                }
            }
        }
    }
}
