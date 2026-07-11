using UnityEngine;

// Bu scripti sahnende herhangi bir objeye (ornegin bos bir "EffectController" objesine) ekle.
// Center alanina efektin merkez alacagi Transform'u (genelde oyuncu) surukle.
//
// Efekti tetiklemenin iki yolu var:
//   A) "radius" alanini direkt world-unit olarak elle degistir (eski yontem, hala calisiyor)
//   B) "SetProgress(0..1)" metodunu cagir - maxRadius'a gore otomatik olceklenir.
//      Bu, DOTween/coroutine ile "efekt merkez noktadan yayiliyor" hissi vermek icin ideal:
//      ornek: DOTween.To(() => progress, x => controller.SetProgress(x), 1f, 2f);
public class WhiteoutRevealController : MonoBehaviour
{
    [Tooltip("Whiteout efektinin merkez alacagi nokta (genelde oyuncu Transform'u)")]
    public Transform center;

    [Tooltip("Beyaz yerine istedigin herhangi bir renk - efekt bu renge dogru gecis yapar")]
    public Color color = Color.white;

    [Header("Manuel kontrol (world unit)")]
    [Tooltip("Efektin yaricapi. SetProgress() kullanmiyorsan bunu direkt degistir.")]
    public float radius = 0f;

    [Header("Progress-tabanli kontrol (0-1)")]
    [Tooltip("Efekt tam yayildiginda ulasacagi maksimum yaricap")]
    public float maxRadius = 30f;

    [Range(0f, 1f)]
    [Tooltip("0 = efekt yok, 1 = maxRadius'a ulasti. SetProgress() ile de degistirebilirsin.")]
    public float progress = 0f;

    [Tooltip("Isaretlenirse progress ters okunur (1 - progress). Efekt merkeze dogru kuculuyor gibi " +
             "gorunuyorsa bunu ac.")]
    public bool invertProgress = false;

    [Tooltip("Kenardaki yumusama mesafesi - sert bir cizgi yerine yumusak gecis icin")]
    public float softness = 3f;

    [Tooltip("Isaretlenirse Progress slider'i radius'u otomatik surer (Inspector'dan canli test icin kullanisli)")]
    public bool driveRadiusFromProgress = true;

    private static readonly int CenterID = Shader.PropertyToID("_WhiteoutCenter");
    private static readonly int RadiusID = Shader.PropertyToID("_WhiteoutRadius");
    private static readonly int SoftnessID = Shader.PropertyToID("_WhiteoutSoftness");
    private static readonly int ColorID = Shader.PropertyToID("_WhiteoutColor");

    void Update()
    {
        if (center == null) return;

        if (driveRadiusFromProgress)
        {
            float effectiveProgress = invertProgress ? (1f - progress) : progress;
            radius = effectiveProgress * maxRadius;
        }

        Shader.SetGlobalVector(CenterID, center.position);
        Shader.SetGlobalFloat(RadiusID, radius);
        Shader.SetGlobalFloat(SoftnessID, softness);
        Shader.SetGlobalColor(ColorID, color);
    }

    // DOTween / coroutine ile kolayca cagirmak icin: controller.SetProgress(0.75f) gibi
    public void SetProgress(float t)
    {
        progress = Mathf.Clamp01(t);
    }

    // Eski API ile uyumluluk icin - direkt world-unit radius istersen
    public void SetRadius(float newRadius)
    {
        driveRadiusFromProgress = false;
        radius = newRadius;
    }
}