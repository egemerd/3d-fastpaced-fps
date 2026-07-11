using UnityEngine;

// Bu scripti sahnende herhangi bir objeye (ornegin bos bir "EffectController" objesine) ekle.
// Center alanina efektin merkez alacagi Transform'u (genelde oyuncu) surukle.
// Radius'u zamanla artirarak (Lerp, DOTween, coroutine ne kullanirsan) reveal efektini tetikle.
public class WhiteoutRevealController : MonoBehaviour
{
    [Tooltip("Whiteout efektinin merkez alacagi nokta (genelde oyuncu Transform'u)")]
    public Transform center;

    [Tooltip("Beyaz alanin yaricapi (world unit). Bunu zamanla artirarak efekti buyut.")]
    public float radius = 0f;

    [Tooltip("Kenardaki yumusama mesafesi - sert bir cizgi yerine yumusak gecis icin")]
    public float softness = 3f;

    private static readonly int CenterID = Shader.PropertyToID("_WhiteoutCenter");
    private static readonly int RadiusID = Shader.PropertyToID("_WhiteoutRadius");
    private static readonly int SoftnessID = Shader.PropertyToID("_WhiteoutSoftness");

    void Update()
    {
        if (center == null) return;

        Shader.SetGlobalVector(CenterID, center.position);
        Shader.SetGlobalFloat(RadiusID, radius);
        Shader.SetGlobalFloat(SoftnessID, softness);
    }

    // Ornek kullanim: bir coroutine veya Tween ile cagirabilirsin
    public void SetRadius(float newRadius)
    {
        radius = newRadius;
    }
}
