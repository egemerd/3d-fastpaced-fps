using UnityEngine;

public class DoorUI : MonoBehaviour
{
    [SerializeField] private GameObject doorSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private Transform closedPosition;
    [SerializeField] private Transform baseOpenPosition;

    private Vector3 enemyDeathPosition;

    DoorController doorController;
    float totalDistance;
    float realDistance;
    float scaleRatio;
    float uiHeight;
    float realHeight;
    

    private bool isDoorOpen = false;
    private Vector3 openPosition;
    private float openPositionY;    
    private void Awake()
    {
        doorController = FindObjectOfType<DoorController>();
    }
    private void Start()
    {
        sliderSpeed = LevelManager.Instance.GetSliderSpeed();
        openPosition = doorSlider.transform.position;
        uiHeight = Vector3.Distance(openPosition, closedPosition.position);
        realHeight = doorController.DistanceBetweenTarget();

        //scaleRatio = uiHeight / realHeight;
        scaleRatio = 0.0145f;

        Debug.Log("[DoorUI] UI Height: " + uiHeight + " | Real Height: " + realHeight + " | Scale Ratio: " + scaleRatio);
    }
    private void OnEnable()
    {
        GameEvents.current.onEnemyDeath += OpenDoor;
    }

    private void OnDisable()
    {
        GameEvents.current.onEnemyDeath -= OpenDoor;
    }
    private void Update()
    {
        DoorMovement();
        //enemyDeathPosition = baseOpenPosition.position;
        Debug.Log("[DoorUI] UI Height: " + uiHeight + " | Real Height: " + realHeight + " | Scale Ratio: " + scaleRatio);
    }

    private void SyncDoorMovement()
    {
        // Gerçek kapının progress'ini al (0 = açık, 1 = kapalı)
        float doorProgress = doorController.GetDoorProgress();

        // UI kapısının hedef pozisyonunu hesapla
        Vector3 targetPosition = Vector3.Lerp(openPosition, closedPosition.position, doorProgress);

        //  Direkt atama yerine smooth hareket
        doorSlider.transform.position = Vector3.MoveTowards(
            doorSlider.transform.position,
            targetPosition,
            0.3f * Time.deltaTime
        );
    }
    private void DoorMovement()
    {
        if(!GameManager.isGameStarted) return;
        float realSliderSpeed = LevelManager.Instance.GetSliderSpeed();
        float uiSpeed = realSliderSpeed * scaleRatio;


        Debug.Log("[DoorUI] ScaleRatio:" + scaleRatio + " | Speed: " + uiSpeed  + " | sliderSpeed: " + realSliderSpeed);
        doorSlider.transform.position += uiSpeed * Vector3.down * Time.deltaTime;
    }

    private void OpenDoor()
    {
        doorSlider.transform.position = baseOpenPosition.position;
    }


    
}
