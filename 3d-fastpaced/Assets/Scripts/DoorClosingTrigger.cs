using UnityEngine;

public class DoorClosingTrigger : MonoBehaviour
{


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ground"))
        {
            Debug.Log("[DoorClosingTrigger] Ground entered the trigger. Closing the door.");
            GameEvents.current.TriggerDoorClosed();
        }
    }
}
