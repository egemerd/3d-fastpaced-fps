using UnityEngine;

public class Player : MonoBehaviour
{
    private void Start()
    {
        CollectibleManager.Instance.RegisterPlayer(GetComponent<PlayerController>() , GetComponent<PlayerHealth>());
    }
    private void OnTriggerEnter(Collider other)
    {
        ICollectibles collectibles = other.GetComponent<ICollectibles>();
        if (collectibles != null)
        {
            collectibles.ApplyEffect();
            Destroy(other.gameObject);
        }
    }
}
