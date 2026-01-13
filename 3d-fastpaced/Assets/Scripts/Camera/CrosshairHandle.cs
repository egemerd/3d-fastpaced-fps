using UnityEngine;

public class CrosshairHandle : MonoBehaviour
{
    PlayerController playerController;
    RectTransform crosshairRect;

    [Header("Shotgun Crosshair Settings")]
    [SerializeField] float maxScale = 3f;
    [SerializeField] float jumpMaxScale = 3f;
    [SerializeField] float jumpMultiplier = 2;
    [SerializeField] float scaleMultipler;
    [SerializeField] float animSpeed;
    Vector2 minScale;

    [Header("Rotation Settings")]
    [SerializeField] private bool enableRotation = true;
    [Tooltip("Sprint sýrasýnda maksimum rotasyon açýsý (derece)")]
    [SerializeField] private float sprintMaxAngle = 15f;
    [Tooltip("Slide sýrasýnda maksimum rotasyon açýsý (derece)")]
    [SerializeField] private float slideMaxAngle = 25f;
    [Tooltip("Hedef açýya ulaþma hýzý")]
    [SerializeField] private float rotateSpeed = 8f;
    [Tooltip("Ýdle/normal harekette açýnýn sýfýra dönüþ hýzý")]
    [SerializeField] private float returnSpeed = 10f;

    [Header("Dynamic Crosshair (Lines)")]
    [Tooltip("Merkez nokta (sabit kalacak)")]
    [SerializeField] private RectTransform centerDot;
    [Tooltip("Üst çizgi")]
    [SerializeField] private RectTransform lineTop;
    [Tooltip("Sað alt çizgi (çapraz)")]
    [SerializeField] private RectTransform lineBottomRight;
    [Tooltip("Sol alt çizgi (çapraz)")]
    [SerializeField] private RectTransform lineBottomLeft;

    [Header("Line Spread Settings")]
    [Tooltip("Normal durumda çizgilerin merkeze olan uzaklýðý")]
    [SerializeField] private float baseSpread = 20f;
    [Tooltip("Hareket ederken maksimum açýlma miktarý")]
    [SerializeField] private float moveSpreadAmount = 15f;
    [Tooltip("Sprint sýrasýnda ekstra açýlma")]
    [SerializeField] private float sprintSpreadMultiplier = 1.5f;
    [Tooltip("Slide sýrasýnda ekstra açýlma")]
    [SerializeField] private float slideSpreadMultiplier = 2f;
    [Tooltip("Havada ekstra açýlma")]
    [SerializeField] private float jumpSpreadAmount = 10f;
    [Tooltip("Çizgilerin hareket yumuþatma hýzý")]
    [SerializeField] private float lineAnimSpeed = 12f;

    private float currentAngle = 0f;
    private float targetAngle = 0f;

    // Her çizgi için baþlangýç pozisyonlarý ve yönleri
    private Vector2 topBaseOffset;
    private Vector2 bottomRightBaseOffset;
    private Vector2 bottomLeftBaseOffset;

    




    public bool isPistol;
    public bool isShotgun;
    private void Awake()
    {
        crosshairRect = GetComponent<RectTransform>();
        playerController = FindObjectOfType<PlayerController>();
    }

    private void OnEnable()
    {
        minScale = crosshairRect.localScale;

        currentAngle = 0f;
        targetAngle = 0f;

        CacheBaseOffsets();
    }

    private void Update()
    {
        if (playerController == null) return;

        
        CrosshairAnim();
        

        if (isPistol)
        {    
            PistolRotationAnim();
            DynamicCrosshairLines();
        }
    }
    private void CrosshairAnim()
    {
        if(isPistol) return;

        bool isSprint = playerController.isSprinting;
        bool isSlide = playerController.isSliding;

        if (!isSprint && !isSlide && playerController.isGrounded)
        {
            crosshairRect.localScale = Vector3.Lerp(
                crosshairRect.localScale,
                minScale,
                Time.deltaTime * animSpeed * 1.5f // Biraz daha hýzlý dönüþ
            );
            return;
        }

        float jump = playerController.isGrounded ? 0 : jumpMultiplier;
        Vector2 jumpOffset = Vector2.one * jump;
        Vector2 targetSize = playerController.GetVelocity() * scaleMultipler * Vector2.one;
        targetSize.x = Mathf.Clamp(targetSize.x, minScale.x, maxScale);
        targetSize.y = Mathf.Clamp(targetSize.y, minScale.y, maxScale);
        //Debug.Log("Target Size:" + targetSize); 
        crosshairRect.localScale = Vector3.Lerp(crosshairRect.localScale, 
            targetSize + jumpOffset, Time.deltaTime * animSpeed);
    }

    private void PistolRotationAnim()
    {
        if (!enableRotation || isShotgun) return;

        bool isSprint = playerController.isSprinting;
        bool isSlide = playerController.isSliding;
        bool isMoving = playerController.isMoving;

        // Hedef açý seçimi - SADECE BELLÝ BÝR DEÐERE ULAÞIR
        if (isSlide)
        {
            targetAngle = slideMaxAngle;
        }
        else if (isSprint)
        {
            targetAngle = sprintMaxAngle;
        }
        else
        {
            // Normal hareket veya idle: sýfýra dön
            targetAngle = 0f;
        }

        // Mevcut açýyý hedefe yumuþakça götür
        float speed = (targetAngle == 0f) ? returnSpeed : rotateSpeed;
        currentAngle = Mathf.Lerp(currentAngle, targetAngle, Time.deltaTime * speed);

        // Rotasyonu uygula
        crosshairRect.localRotation = Quaternion.Euler(0f, 0f, currentAngle);

    }

    private void DynamicCrosshairLines()
    {
        if (lineTop == null || lineBottomRight == null || lineBottomLeft == null)
            return;

        // Hareket durumuna göre spread hesapla
        float targetSpread = CalculateTargetSpread();

        // Her çizgiyi kendi yönünde hareket ettir
        UpdateLinePosition(lineTop, topBaseOffset, targetSpread);
        UpdateLinePosition(lineBottomRight, bottomRightBaseOffset, targetSpread);
        UpdateLinePosition(lineBottomLeft, bottomLeftBaseOffset, targetSpread);
    }

    private float CalculateTargetSpread()
    {
        bool isMoving = playerController.isMoving;
        bool isSprint = playerController.isSprinting;
        bool isSlide = playerController.isSliding;
        bool inAir = !playerController.isGrounded;

        // Base spread
        float spread = 0f;

        // Hareket spread'i
        if (isMoving)
        {
            float velocity = playerController.GetVelocity();
            spread += velocity * moveSpreadAmount * 0.1f; // Hýz bazlý spread
        }

        // Sprint çarpaný
        if (isSprint)
        {
            spread *= sprintSpreadMultiplier;
        }

        // Slide çarpaný (en fazla açýlma)
        if (isSlide)
        {
            spread *= slideSpreadMultiplier;
        }

        // Havada ekstra açýlma
        if (inAir)
        {
            spread += jumpSpreadAmount;
        }

        // Minimum ve maksimum sýnýrla
        spread = Mathf.Clamp(spread, 0f, 50f);

        return spread;
    }

    private void CacheBaseOffsets()
    {
        if (lineTop != null)
            topBaseOffset = lineTop.anchoredPosition;
        if (lineBottomRight != null)
            bottomRightBaseOffset = lineBottomRight.anchoredPosition;
        if (lineBottomLeft != null)
            bottomLeftBaseOffset = lineBottomLeft.anchoredPosition;
    }
    private void UpdateLinePosition(RectTransform line, Vector2 baseOffset, float spreadAmount)
    {
        // Yön vektörü (merkeze göre)
        Vector2 direction = baseOffset.normalized;

        // Eðer baseOffset sýfýr ise fallback yön ver
        if (direction == Vector2.zero)
            direction = Vector2.up;

        // Hedef pozisyon = base offset + (yön * spread)
        Vector2 targetPosition = baseOffset + (direction * spreadAmount);

        // Yumuþak geçiþ
        line.anchoredPosition = Vector2.Lerp(
            line.anchoredPosition,
            targetPosition,
            Time.deltaTime * lineAnimSpeed
        );
    }
}

