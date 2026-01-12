using UnityEngine;

public class CollectibleAnimation : MonoBehaviour
{
    [Header("Float Animation")]
    [Tooltip("Hareket hýzý (yüksek deðer = hýzlý hareket)")]
    [SerializeField] private float floatSpeed = 1f;

    [Tooltip("Yukarý-aþaðý hareket mesafesi")]
    [SerializeField] private float floatAmplitude = 0.5f;

    [Header("Rotation Animation (Optional)")]
    [Tooltip("Y ekseninde dönme hýzý (0 = dönme yok)")]
    [SerializeField] private float rotationSpeed = 50f;

    [Header("Animation Settings")]
    [Tooltip("Baþlangýç offset'i (her item farklý fazda baþlar)")]
    [SerializeField] private bool randomStartOffset = true;

    [Tooltip("Smooth interpolation tipi")]
    [SerializeField] private AnimationType animationType = AnimationType.Sine;

    private Vector3 startPosition;
    private float timeOffset;

    public enum AnimationType
    {
        Sine,           // Yumuþak sinüs hareketi
        SmoothStep,     // Ease-in-out hareket
        EaseInOut       // Yumuþak geçiþ
    }

    private void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        startPosition = transform.position;

        // Random offset (her collectible farklý fazda baþlasýn)
        if (randomStartOffset)
        {
            timeOffset = Random.Range(0f, Mathf.PI * 2f);
        }
    }

    private void Update()
    {
        // Yukarý-aþaðý hareket
        FloatMovement();

        // Dönme animasyonu
        if (rotationSpeed > 0)
        {
            RotateAnimation();
        }
    }

    private void FloatMovement()
    {
        float newY = 0f;
        float time = Time.time * floatSpeed + timeOffset;

        switch (animationType)
        {
            case AnimationType.Sine:
                // Klasik sinüs hareketi
                newY = Mathf.Sin(time) * floatAmplitude;
                break;

            case AnimationType.SmoothStep:
                // Smooth step (ease-in-out)
                float t = Mathf.PingPong(time, 1f);
                newY = Mathf.SmoothStep(-floatAmplitude, floatAmplitude, t);
                break;

            case AnimationType.EaseInOut:
                // Ease-in-out cubic
                float pingPong = Mathf.PingPong(time, 1f);
                float eased = EaseInOutCubic(pingPong);
                newY = Mathf.Lerp(-floatAmplitude, floatAmplitude, eased);
                break;
        }

        // Yeni pozisyonu uygula
        transform.position = startPosition + new Vector3(0f, newY, 0f);
    }

    private void RotateAnimation()
    {
        // Y ekseninde sabit hýzda dön
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }

    // Cubic ease-in-out fonksiyonu
    private float EaseInOutCubic(float t)
    {
        return t < 0.5f
            ? 4f * t * t * t
            : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }

    // Gizmos ile editor'da hareket aralýðýný göster
    private void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = Color.yellow;
        // Alt sýnýr
        Gizmos.DrawWireSphere(pos + Vector3.down * floatAmplitude, 0.1f);
        // Üst sýnýr
        Gizmos.DrawWireSphere(pos + Vector3.up * floatAmplitude, 0.1f);
        // Baþlangýç noktasý
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.15f);

        // Hareket çizgisi
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos + Vector3.down * floatAmplitude, pos + Vector3.up * floatAmplitude);
    }
}
