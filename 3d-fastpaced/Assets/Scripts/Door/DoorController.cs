using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorSlider;
    private float sliderSpeed;
    [SerializeField] private float slideDistance;
    [SerializeField] private Transform closedPosition;

    [SerializeField] private float loadSceneDuration = 2f;


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
        Debug.Log("[DoorController] sliderSpeed: " + sliderSpeed);
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

    public float DistanceBetweenTarget()
    {
        float distance = Vector3.Distance(openPosition, closedPosition.position);
        return distance;
    }

    public float GetSliderSpeed()
    {
        return sliderSpeed;
    }
    public float GetScaleY()
    {
        return doorSlider.transform.localScale.y;
    }
    private void DoorMovement()
    {
        
        sliderSpeed = LevelManager.Instance.GetSliderSpeed();
        


        Vector3 targetPosition = isDoorOpen ? openPosition : closedPosition.position;
        doorSlider.transform.position += sliderSpeed * Vector3.down * Time.deltaTime;
    }

    private void OpenDoor()
    {
        Debug.Log("Door Opening");
        doorSlider.transform.position = openPosition;
    }
    public float GetDoorProgress()
    {
        float currentDistance = Vector3.Distance(doorSlider.transform.position, closedPosition.position);
        float totalDistance = Vector3.Distance(openPosition, closedPosition.position);

        // Ýlerleme = (Toplam mesafe - Kalan mesafe) / Toplam mesafe
        float progress = 1f - (currentDistance / totalDistance);

        return Mathf.Clamp01(progress);
    }

    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Player entered door trigger");
        if (!other.CompareTag("Player")) return;
        //LevelManager.Instance.ActivateLevelEndCanvas();
    }

    


}
