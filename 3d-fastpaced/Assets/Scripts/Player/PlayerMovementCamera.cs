using UnityEngine;

public class PlayerMovementCamera : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera mainCamera;

    [Header("Sliding Tilt Settings")]
    [SerializeField] private float slideTiltAngle = 8f;      // Hedef eðim açýsý (Z ekseni - roll)
    [SerializeField] private float tiltLerpSpeed = 8f;       // Tilt'in ne hýzda açýlýp kapanacaðý

    [Header("Sliding Shake Settings")]
    [SerializeField] private float shakeAmplitude = 1.2f;    // Shake'in max genliði (derece)
    [SerializeField] private float shakeFrequency = 3f;      // Shake'in hýzý (Perlin noise sample hýzý)

    // --- Internal state ---
    private bool isSliding = false;
    private float currentTiltBlend = 0f;   // 0 = düz, 1 = tam eðik. Enter/Exit arasý yumuþak geçiþ için
    private float shakeSeedX;              // Her eksen için farklý Perlin seed'i (ayný noise'u kullanmamak için)
    private float shakeSeedY;

    private void Start()
    {
        // Perlin noise'un X ve Y ekseninde birbirinden baðýmsýz görünmesi için
        // farklý baþlangýç noktalarýndan örnekliyoruz.
        shakeSeedX = Random.Range(0f, 100f);
        shakeSeedY = Random.Range(0f, 100f);
    }

    private void Update()
    {
        // Slide state'inde olsak da olmasak da her frame blend'i günceller,
        // güncelledikten sonra da kamerayý buna göre pozisyonlarýz.
        
    }

    public void StartSlidingCamera()
    {
        isSliding = true;
    }

    public void StopSlidingCamera()
    {
        isSliding = false;
    }

    public void UpdateTiltBlend()
    {
        // isSliding true ise blend 1'e, false ise 0'a doðru yumuþakça ilerler.
        float target = 1f;

        // Frame-rate baðýmsýz exponential smoothing.
        // Vector3.Lerp(x, y, sabit_deger * Time.deltaTime) FPS'e baðýmlý yanlýþ bir yaklaþýmdýr,
        // bunun yerine bu formül kullanýlýr: her frame farkýn belli bir yüzdesini kapatýr.
        float t = 1f - Mathf.Exp(-tiltLerpSpeed * Time.deltaTime);
        currentTiltBlend = Mathf.Lerp(currentTiltBlend, target, t);
    }

    public void LerpCamera()
    {
        // 1) BASE TILT: blend'e göre 0 ile slideTiltAngle arasýnda bir Z rotasyonu
        float tiltZ = currentTiltBlend * slideTiltAngle;

        // 2) SHAKE: sadece blend > 0 iken anlamlý, blend ile çarparak
        // slide bitince shake'in de otomatik sönmesini saðlýyoruz.
        // Random.Range yerine Perlin noise kullanýyoruz çünkü Perlin zaman içinde
        // SÜREKLÝ bir eðri üretir (ardýþýk frame'ler arasýnda ani sýçrama olmaz),
        // Random.Range ise her frame'de alakasýz bir deðer verip "titreþim" deðil "gürültü" yaratýr.
        float noiseX = (Mathf.PerlinNoise(shakeSeedX, Time.time * shakeFrequency) - 0.5f) * 2f;
        float noiseY = (Mathf.PerlinNoise(shakeSeedY, Time.time * shakeFrequency) - 0.5f) * 2f;

        float shakeX = noiseX * shakeAmplitude * currentTiltBlend;
        float shakeY = noiseY * shakeAmplitude * currentTiltBlend;

        // 3) Kameranýn orijinal (parent'tan gelen / diðer scriptlerden gelen) local rotasyonunu bozmamak için
        // sadece kendi katmanýmýzý ayrý bir Vector3 olarak tutup, bunu localRotation'a
        // Euler açýsý olarak set ediyoruz. Burada localEulerAngles yerine Quaternion.Euler kullanmak
        // wrap-around (359->0) sorununu tamamen ortadan kaldýrýr çünkü biz zaten
        // 0'dan baþlayan temiz bir açý hesaplýyoruz, var olan açýyý okuyup üstüne yazmýyoruz.
        Quaternion tiltRotation = Quaternion.Euler(shakeX, shakeY, tiltZ);

        // Eðer bu kamerada baþka bir script (örneðin normal look/mouse rotation) de
        // rotasyonu kontrol ediyorsa, bu satýrý o scriptin ÜZERÝNE bir "ek katman" olarak
        // ayrý bir child objede yapmanýz daha güvenlidir (aþaðýda not var).
        mainCamera.transform.localRotation = tiltRotation;
    }
}