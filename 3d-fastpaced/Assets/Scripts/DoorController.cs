using UnityEngine;

public class DoorController : MonoBehaviour
{
    [SerializeField] private GameObject doorSlider;
    [SerializeField] private float sliderSpeed;
    [SerializeField] private float slideDistance;
    [SerializeField] private Transform closedPosition;


    private bool isDoorOpen = false;    
    private bool canDoorSlide = false;
    private Vector3 openPosition;


    private void Start()
    {
        openPosition = doorSlider.transform.position;
    }
    private void Update()
    {
        if(canDoorSlide && Input.GetKeyDown(KeyCode.E))
        {
            isDoorOpen = !isDoorOpen;
        }

        Vector3 targetPosition = isDoorOpen ? openPosition : closedPosition.position;
        doorSlider.transform.position = Vector3.MoveTowards
            (doorSlider.transform.position, targetPosition, 
            sliderSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        canDoorSlide = true;
    }

    private void OnTriggerExit(Collider other)
    {
        canDoorSlide = false;
    }

    
}
