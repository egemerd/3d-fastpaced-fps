using UnityEngine;

public class DoorClosingTrigger : MonoBehaviour
{
    public bool canCloseDoor;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground") && canCloseDoor)
        {
            Debug.Log("[DoorClosingTrigger] Ground entered the trigger. Closing the door.");
            GameEvents.current.TriggerDoorClosed();
        }
    }
}
