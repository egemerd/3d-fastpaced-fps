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
    }
    private void Update()
    {
        DoorMovement(); 
    }

    private void OnEnable()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyDeath += OpenDoor;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyDeath -= OpenDoor;
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
        doorSlider.transform.position = openPosition;
    }

    
}
