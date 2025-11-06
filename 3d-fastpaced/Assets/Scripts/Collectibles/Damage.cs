using UnityEngine;

public class Damage : MonoBehaviour, ICollectibles
{
    [SerializeField] private float damageAmount = 10f;

    public void ApplyEffect()
    {
        CollectibleManager.Instance.ApplyDamage(damageAmount);
        Debug.Log("Damage");
    }
}
