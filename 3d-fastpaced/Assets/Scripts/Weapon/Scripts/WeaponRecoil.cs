using UnityEngine;
using System.Collections;

public class WeaponRecoil : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform weaponModelTransform; // silahýn kendi transform'u (kick uygulanacak)

    [Header("Visual Kick - Position")]
    [SerializeField] private Vector3 kickBackPosition = new Vector3(0f, 0f, -0.15f); // silahýn geri gidiþ miktarý
    [SerializeField] private float kickPositionSnapSpeed = 25f;   // geri gidiþ hýzý (yüksek = anlýk/snappy)
    [SerializeField] private float returnPositionSpeed = 8f;      // eski konuma dönüþ hýzý

    [Header("Visual Kick - Rotation")]
    [SerializeField] private float kickRotationPitch = 15f;   // silahýn yukarý kalkma açýsý (derece)
    [SerializeField] private float kickRotationRandomYaw = 4f; // saða/sola rastgele sapma
    [SerializeField] private float kickRotationSnapSpeed = 30f;
    [SerializeField] private float returnRotationSpeed = 10f;

    [Header("Overshoot / Sekme (Cartoonish His)")]
    [SerializeField] private float overshootAmount = 1.3f; // geri dönerken biraz fazla gidip sekmesi için çarpan
    [SerializeField] private float overshootSpeed = 15f;

    [Header("Gerçek Recoil - Niþan Sapmasý")]
    [SerializeField] private float aimPunchPitch = 1.2f;   // her atýþta kameranýn/niþanýn YUKARI kayma miktarý (derece)
    [SerializeField] private float aimPunchYawRandom = 0.6f; // her atýþta rastgele sað/sol kayma
    [SerializeField] private float aimRecoverySpeed = 6f;    // niþan sapmasýnýn ne kadar hýzlý toparlandýðý

    [Header("Sürekli Ateþ Pattern'i")]
    [SerializeField] private float maxPatternMultiplier = 2.2f;
    [SerializeField] private float patternGrowthPerShot = 0.18f;
    [SerializeField] private float patternResetDelay = 0.15f;
    [SerializeField] private float patternResetSpeed = 4f;

    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;

    private Vector3 currentKickPosOffset;
    private Vector3 currentKickRotOffset; // Euler olarak tutuyoruz, uygulamada Quaternion'a çeviriyoruz

    private float currentPatternMultiplier = 1f;
    private float lastFireTime;

    // Gerçek recoil - niþaný etkileyen deðerler (dýþarýdan okunacak)
    public float CurrentAimPitchOffset { get; private set; }
    public float CurrentAimYawOffset { get; private set; }

    private void Awake()
    {
        if (weaponModelTransform == null) weaponModelTransform = transform;
        initialLocalPosition = weaponModelTransform.localPosition;
        initialLocalRotation = weaponModelTransform.localRotation;
    }

    private void Update()
    {
        // Pattern reset (tetik býrakýlýnca)
        if (Time.time - lastFireTime > patternResetDelay && currentPatternMultiplier > 1f)
        {
            currentPatternMultiplier = Mathf.MoveTowards(currentPatternMultiplier, 1f, patternResetSpeed * Time.deltaTime);
        }

        // Niþan sapmasýnýn yavaþça sýfýra dönmesi (gerçek recoil recovery)
        CurrentAimPitchOffset = Mathf.MoveTowards(CurrentAimPitchOffset, 0f, aimRecoverySpeed * Time.deltaTime);
        CurrentAimYawOffset = Mathf.MoveTowards(CurrentAimYawOffset, 0f, aimRecoverySpeed * Time.deltaTime);

        // Silah pozisyonunu hedefe doðru yumuþat (spring benzeri iki aþamalý lerp)
        currentKickPosOffset = Vector3.Lerp(currentKickPosOffset, Vector3.zero, returnPositionSpeed * Time.deltaTime);
        currentKickRotOffset = Vector3.Lerp(currentKickRotOffset, Vector3.zero, returnRotationSpeed * Time.deltaTime);

        weaponModelTransform.localPosition = initialLocalPosition + currentKickPosOffset;
        weaponModelTransform.localRotation = initialLocalRotation * Quaternion.Euler(currentKickRotOffset);
    }

    public void ApplyRecoil()
    {
        lastFireTime = Time.time;
        currentPatternMultiplier = Mathf.Min(currentPatternMultiplier + patternGrowthPerShot, maxPatternMultiplier);

        // --- Görsel kick: anýnda hedefe zýpla (snap), sonra Update() içinde yumuþakça geri dönecek ---
        float randomYawSign = Random.Range(-1f, 1f);

        currentKickPosOffset = kickBackPosition * currentPatternMultiplier;
        currentKickRotOffset = new Vector3(
            -kickRotationPitch * currentPatternMultiplier,          // yukarý kalkma (X ekseni negatif = yukarý bakma, modeline göre iþareti kontrol et)
            kickRotationRandomYaw * randomYawSign * currentPatternMultiplier,
            0f
        );

        // --- Gerçek recoil: niþaný da kaydýr ---
        CurrentAimPitchOffset += aimPunchPitch * currentPatternMultiplier;
        CurrentAimYawOffset += Random.Range(-aimPunchYawRandom, aimPunchYawRandom) * currentPatternMultiplier;
    }

    

    public void ResetPattern()
    {
        currentPatternMultiplier = 1f;
    }
}