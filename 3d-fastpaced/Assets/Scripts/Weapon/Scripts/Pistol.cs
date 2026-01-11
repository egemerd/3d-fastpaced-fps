using UnityEngine;
using System.Collections;

public class Pistol : Gun
{
    [SerializeField] private float shootAnimDuration = 0.08f;
    [SerializeField] private float returnShootAnimDuration = 0.25f;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilPositionY = 0.08f;      // Yukarı hareket (artırıldı)
    [SerializeField] private float recoilPositionZ = -0.2f;      // Geriye hareket (artırıldı)
    [SerializeField] private float recoilRotationX = -25f;       // Yukarı rotasyon (artırıldı)
    [SerializeField] private float recoilRotationZ = 2f;         // Hafif yan yatış (YENİ)
    [SerializeField] private AnimationCurve recoilCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AnimationCurve returnCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);


    [Header("Sway Settings")]
    [Tooltip("Maksimum sway mesafesi (units)")]
    [SerializeField] private float maxSwayOffset = 0.1f;
    [Tooltip("Sway yumuşatma hızı")]
    [SerializeField] private float swaySmoothing = 10f;

    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shootAnimationCoroutine;

    // Sway için
    private Vector3 currentSwayOffset = Vector3.zero;
    private Vector3 targetSwayOffset = Vector3.zero;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    public override void Update()
    {
        base.Update();

        // Sway her frame güncellenir
        UpdateSway();

        // Input handling
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleReload();
        }
    }
    private void OnDisable()
    {
        //  Silah deaktif olduğunda tüm animasyonları durdur ve orijinal konuma dön
        if (shootAnimationCoroutine != null)
        {
            StopCoroutine(shootAnimationCoroutine);
            shootAnimationCoroutine = null;
        }

        // Sway offset'lerini sıfırla
        currentSwayOffset = Vector3.zero;
        targetSwayOffset = Vector3.zero;

        // Orijinal pozisyon ve rotasyona dön
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }
    public override void Shoot()
    {
        RaycastHit hit;

        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, gunData.fireRange))
        {
            StartBulletFire(hit.point, hit);
            Debug.Log("Pistol hit: " + hit.collider.name);
            if (hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Pistol hit enemy: " + hit.collider.name);
                
                Destroy(hit.collider.gameObject);
            }
        }
        else
        {
            StartBulletFire(hit.point, hit);
        }

            WeaponShootAnimation();
    }

    public override void WeaponShootAnimation()
    {
        if (shootAnimationCoroutine != null)
        {
            StopCoroutine(shootAnimationCoroutine);
        }

        shootAnimationCoroutine = StartCoroutine(ShootAnimationCoroutine());
    }

    /// <summary>
    /// Her frame sway offset'ini günceller
    /// </summary>
    private void UpdateSway()
    {
        // Input al
        float horizontal = Input.GetAxisRaw("Horizontal");

        // Hedef sway offset (TERS yön)
        if (horizontal < 0f) // A (sola hareket)
        {
            targetSwayOffset = new Vector3(maxSwayOffset, 0f, 0f); // Silah sağa kayar
        }
        else if (horizontal > 0f) // D (sağa hareket)
        {
            targetSwayOffset = new Vector3(-maxSwayOffset, 0f, 0f); // Silah sola kayar
        }
        else
        {
            targetSwayOffset = Vector3.zero;
        }

        // Smooth geçiş
        currentSwayOffset = Vector3.Lerp(currentSwayOffset, targetSwayOffset, Time.deltaTime * swaySmoothing);

        // ✅ Shoot animasyonu yokken pozisyonu güncelle
        if (shootAnimationCoroutine == null)
        {
            transform.localPosition = originalPosition + currentSwayOffset;
        }
    }

    private IEnumerator ShootAnimationCoroutine()
    {
        // ===== RECOIL PHASE =====
        float recoilDuration = shootAnimDuration;
        float elapsed = 0f;

        // ✅ Shoot başladığında mevcut sway offset'i kaydet
        Vector3 shootStartSwayOffset = currentSwayOffset;

        // Recoil hedef pozisyonu (sway DAHIL)
        Vector3 recoilPosition = originalPosition + shootStartSwayOffset + new Vector3(
            Random.Range(-0.02f, 0.02f),
            recoilPositionY,
            recoilPositionZ
        );

        Quaternion recoilRotation = originalRotation * Quaternion.Euler(
            recoilRotationX,
            Random.Range(-1f, 1f),
            recoilRotationZ
        );

        // Başlangıç pozisyonu (sway dahil mevcut pozisyon)
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        // Recoil animasyonu
        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoilDuration;
            float curveT = recoilCurve.Evaluate(t);

            transform.localPosition = Vector3.Lerp(startPos, recoilPosition, curveT);
            transform.localRotation = Quaternion.Slerp(startRot, recoilRotation, curveT);

            yield return null;
        }

        transform.localPosition = recoilPosition;
        transform.localRotation = recoilRotation;

        // ===== RETURN PHASE =====
        float returnDuration = returnShootAnimDuration;
        elapsed = 0f;

        startPos = transform.localPosition;
        startRot = transform.localRotation;

        // Return animasyonu - orijinal + GÜNCEL sway offset'e dön
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;
            float curveT = returnCurve.Evaluate(t);

            // ✅ Return sırasında güncel sway offset'i kullan (A/D hala basılı olabilir)
            Vector3 targetReturnPos = originalPosition + currentSwayOffset;

            transform.localPosition = Vector3.Lerp(startPos, targetReturnPos, curveT);
            transform.localRotation = Quaternion.Slerp(startRot, originalRotation, curveT);

            yield return null;
        }

        // ✅ Final pozisyon: original + güncel sway
        transform.localPosition = originalPosition + currentSwayOffset;
        transform.localRotation = originalRotation;

        shootAnimationCoroutine = null;
    }

    public void ResetWeaponPosition()
    {
        if (shootAnimationCoroutine != null)
        {
            StopCoroutine(shootAnimationCoroutine);
            shootAnimationCoroutine = null;
        }

        currentSwayOffset = Vector3.zero;
        targetSwayOffset = Vector3.zero;
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }

}
