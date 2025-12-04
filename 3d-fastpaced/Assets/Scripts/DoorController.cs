using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private float slideDistance;
    [SerializeField] private Transform closedPosition;


    private bool isDoorOpen = false;    
    private Vector3 openPosition;


    private void Start()
    {
        openPosition = doorSlider.transform.position;
        Debug.Log("DoorController OnEnable çalýþtý.");

        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyDeath += OpenDoor;
            // Abonelik baþarýlý oldu mu? Kontrol 2:
            Debug.Log("Kapý, onEnemyDeath eventine baþarýyla abone oldu.");
        }
        else
        {
            // Event sistemi hazýr deðil! Kontrol 3: Sorun burada olabilir.
            Debug.LogError("HATA: GameEvents.current null! Abonelik baþarýsýz.");
        }
    }
    private void Update()
    {
        DoorMovement();
        if (Input.GetKeyDown(KeyCode.E))
        {
            
            OpenDoor();
        }
    }

    private void OnEnable()
    {
        // DoorController.OnEnable çalýþýyor mu? Kontrol 1:
        
    }

    private void OnDisable()
    {
        // DoorController.OnEnable çalýþýyor mu? Kontrol 1:
        Debug.Log("DoorController OnDisnable çalýþtý.");

        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyDeath -= OpenDoor;
            // Abonelik baþarýlý oldu mu? Kontrol 2:
            Debug.Log("Kapý, onEnemyDeath eventine baþarýyla abone oldu.");
        }
        else
        {
            // Event sistemi hazýr deðil! Kontrol 3: Sorun burada olabilir.
            Debug.LogError("HATA: GameEvents.current null! Abonelik baþarýsýz.");
        }
    }

    private void DoorMovement()
    {
        Vector3 targetPosition = isDoorOpen ? openPosition : closedPosition.position;
        doorSlider.transform.position = Vector3.MoveTowards
            (doorSlider.transform.position, targetPosition,
            sliderSpeed * Time.deltaTime);
    }

    private void OpenDoor()
    {
        Debug.Log("Door Opening");
        doorSlider.transform.position = openPosition;
    }

    
}
