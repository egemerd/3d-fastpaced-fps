using UnityEngine;

public class DoorUI : MonoBehaviour
{
    [SerializeField] private GameObject doorSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private Transform closedPosition;

    DoorController doorController;
    float totalDistance;
    float realDistance;
    float scaleRatio;

    private bool isDoorOpen = false;
    private Vector3 openPosition;

    private void Awake()
    {
        doorController = FindObjectOfType<DoorController>();
    }
    private void Start()
    {
        sliderSpeed = LevelManager.Instance.GetSliderSpeed();
        openPosition = doorSlider.transform.position;
        float uiHeight = Vector3.Distance(openPosition, closedPosition.position);
        float realHeight = doorController.DistanceBetweenTarget();

        scaleRatio = uiHeight / realHeight;

              
    }

    private void Update()
    {
        DoorMovement();
    }

    private void SyncDoorMovement()
    {
        // Gerçek kapýnýn progress'ini al (0 = açýk, 1 = kapalý)
        float doorProgress = doorController.GetDoorProgress();

        // UI kapýsýnýn hedef pozisyonunu hesapla
        Vector3 targetPosition = Vector3.Lerp(openPosition, closedPosition.position, doorProgress);

        //  Direkt atama yerine smooth hareket
        doorSlider.transform.position = Vector3.MoveTowards(
            doorSlider.transform.position,
            targetPosition,
            doorController.GetSliderSpeed() * Time.deltaTime
        );
    }
    private void DoorMovement()
    {
        float realSliderSpeed = doorController.GetSliderSpeed();
        float uiSpeed = realSliderSpeed * scaleRatio;


        Debug.Log("UI Distance: " + totalDistance + " | Real Distance: " + realDistance + " | Speed: "  + " | sliderSpeed: " + doorController.GetSliderSpeed());
        doorSlider.transform.position += uiSpeed * Vector3.down * Time.deltaTime;
    }


    
}
