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


    private Vector3 originalPosition;
    private Quaternion originalRotation;
    private Coroutine shootAnimationCoroutine;

    private void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
    public override void Update()
    {
        base.Update();
        if (Input.GetButtonDown("Fire1"))
        {
            TryShoot();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            HandleReload();
        }
    }
    public override void Shoot()
    {
        RaycastHit hit;  
        if (Physics.Raycast(mainCamera.position, mainCamera.forward, out hit, gunData.fireRange))
        {
            Debug.Log("Pistol hit: " + hit.collider.name);
            if(hit.collider.CompareTag("Enemy"))
            {
                Debug.Log("Pistol hit enemy: " + hit.collider.name);
                Destroy(hit.collider.gameObject);
            }   
            
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

    private IEnumerator ShootAnimationCoroutine()
    {
        float recoilDuration = shootAnimDuration;
        float elapsed = 0f;

        //  DAHA DRAMATIK hedef değerler
        Vector3 recoilPosition = originalPosition + new Vector3(
            Random.Range(-0.02f, 0.02f),  // Hafif rastgele yan hareket (YENİ)
            recoilPositionY,               // Yukarı (0.05 → 0.08)
            recoilPositionZ                // Geriye (0.15 → 0.2)
        );

        // DAHA FAZLA rotasyon + hafif roll
        Quaternion recoilRotation = originalRotation * Quaternion.Euler(
            recoilRotationX,               // Yukarı ()
            Random.Range(-1f, 1f),         // Hafif rastgele yön (YENİ)
            recoilRotationZ                // Yan yatış (YENİ)
        );

        // Başlangıç değerleri
        Vector3 startPos = transform.localPosition;
        Quaternion startRot = transform.localRotation;

        //  Recoil animasyonu - Custom curve ile
        while (elapsed < recoilDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / recoilDuration;

            // AnimationCurve kullan (Inspector'dan özelleştirilebilir)
            float curveT = recoilCurve.Evaluate(t);

            // Pozisyon interpolasyonu
            transform.localPosition = Vector3.Lerp(startPos, recoilPosition, curveT);

            // Rotasyon interpolasyonu
            transform.localRotation = Quaternion.Slerp(startRot, recoilRotation, curveT);

            yield return null;
        }

        // Kesin değerlere ayarla
        transform.localPosition = recoilPosition;
        transform.localRotation = recoilRotation;

        // ===== RETURN PHASE (Geri dönüş) - SMOOTH =====
        float returnDuration = returnShootAnimDuration;
        elapsed = 0f;

        startPos = transform.localPosition;
        startRot = transform.localRotation;

        // Return animasyonu - Custom curve ile
        while (elapsed < returnDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / returnDuration;

            // AnimationCurve kullan
            float curveT = returnCurve.Evaluate(t);

            // Pozisyon interpolasyonu - orijinale dön
            transform.localPosition = Vector3.Lerp(startPos, originalPosition, curveT);

            // Rotasyon interpolasyonu - orijinale dön
            transform.localRotation = Quaternion.Slerp(startRot, originalRotation, curveT);

            yield return null;
        }

        // Kesin orijinal değerlere dön
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;

        shootAnimationCoroutine = null;
    }

    // Reset fonksiyonu - gerekirse manuel reset için
    public void ResetWeaponPosition()
    {
        if (shootAnimationCoroutine != null)
        {
            StopCoroutine(shootAnimationCoroutine);
            shootAnimationCoroutine = null;
        }

        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
    }


}
