using UnityEngine;

public class DoorUIAnimation : MonoBehaviour
{
    [Header("Sway Settings")]
    [SerializeField] private float swayAmountY = 10f;  // Y ekseni için sway miktarý
    [SerializeField] private float swayAmountZ = 10f;  // Z ekseni için sway miktarý
    [SerializeField] private float swaySmooth = 8f;
    [SerializeField] private float maxSwayAngleY = 20f;
    [SerializeField] private float maxSwayAngleZ = 20f;

    private Vector3 baseRotation = new Vector3(0f, -90f, 0f);  // Baþlangýç rotasyonu
    private Vector3 targetRotation;
    private Vector3 currentRotation;
    private Vector2 mouseInput;
    private Vector2 accumulatedSway;  // Biriken sway deðeri

    private void Start()
    {
        currentRotation = baseRotation;
        targetRotation = baseRotation;
        accumulatedSway = Vector2.zero;
        transform.localRotation = Quaternion.Euler(baseRotation);
    }

    private void Update()
    {
        GetMouseInput();
        CalculateSway();
        ApplySway();
    }

    private void GetMouseInput()
    {
        // InputManager'dan lookInput'u al (Singleton pattern ile)
        if (InputManager.Instance != null)
        {
            mouseInput = InputManager.Instance.lookInput;
        }
    }

    private void CalculateSway()
    {
        // Mouse hareketini biriktir (pozisyonu koru)
        accumulatedSway.x += mouseInput.x * Time.deltaTime;
        accumulatedSway.y += mouseInput.y * Time.deltaTime;

        // Biriken sway deðerlerini hesapla
        float swayY = -accumulatedSway.x * swayAmountY;
        float swayZ = -accumulatedSway.y * swayAmountZ;

        // Sway açýlarýný sýnýrla
        swayY = Mathf.Clamp(swayY, -maxSwayAngleY, maxSwayAngleY);
        swayZ = Mathf.Clamp(swayZ, -maxSwayAngleZ, maxSwayAngleZ);

        // Clamp sonrasý accumulatedSway'i güncelle (limitlerin dýþýna çýkmasýn)
        accumulatedSway.x = -swayY / swayAmountY;
        accumulatedSway.y = -swayZ / swayAmountZ;

        // Target rotation = Base rotation + Sway offset
        targetRotation = new Vector3(
            baseRotation.x,
            baseRotation.y + swayY,
            baseRotation.z + swayZ
        );
    }

    private void ApplySway()
    {
        // Smooth geçiþ ile rotasyonu uygula (Balatro tarzý smooth hareket)
        currentRotation = Vector3.Lerp(currentRotation, targetRotation, Time.deltaTime * swaySmooth);

        // Local rotation'ý ayarla
        transform.localRotation = Quaternion.Euler(currentRotation);
    }
}