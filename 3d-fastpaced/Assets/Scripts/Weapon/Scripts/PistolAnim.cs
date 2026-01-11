using UnityEngine;

public class PistolAnim : MonoBehaviour
{
    [Header("Sway Settings")]
    [Tooltip("Maksimum sway mesafesi (units)")]
    [SerializeField] private float maxSwayOffset = 0.1f;
    [Tooltip("Sway yumuþatma hýzý")]
    [SerializeField] private float swaySmoothing = 8f;

    private Vector3 originalPosition;
    private Vector3 currentSwayOffset = Vector3.zero;
    private Vector3 targetSwayOffset = Vector3.zero;

    private void Start()
    {
        // Baþlangýç pozisyonunu kaydet
        originalPosition = transform.localPosition;
    }

    private void Update()
    {
        HandleSway();
    }

    /// <summary>
    /// A/D tuþlarýna göre silahýn pozisyonunu saða/sola kaydýrýr (sway)
    /// </summary>
    private void HandleSway()
    {
        // Input al (A = -1, D = +1)
        float horizontal = Input.GetAxisRaw("Horizontal"); // A/D veya Sol/Sað ok tuþlarý

        // Hedef sway offset'ini hesapla (TERS yön: A basýnca saða, D basýnca sola)
        if (horizontal < 0f) // A tuþu (sola hareket)
        {
            targetSwayOffset = new Vector3(maxSwayOffset, 0f, 0f); // Silah SAÐA kayar (ters momentum)
        }
        else if (horizontal > 0f) // D tuþu (saða hareket)
        {
            targetSwayOffset = new Vector3(-maxSwayOffset, 0f, 0f); // Silah SOLA kayar (ters momentum)
        }
        else // Tuþ basýlý deðil
        {
            targetSwayOffset = Vector3.zero; // Orijinal pozisyona dön
        }

        // Mevcut offset'i hedefe yumuþakça götür
        currentSwayOffset = Vector3.Lerp(currentSwayOffset, targetSwayOffset, Time.deltaTime * swaySmoothing);

        // Pozisyonu uygula (orijinal pozisyon + sway offset)
        transform.localPosition = originalPosition + currentSwayOffset;
    }
}
