using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    public static CollectibleManager Instance { get; private set; }

    private PlayerController playerController;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        Instance = this;
    }


    public void RegisterPlayer(PlayerController controller , PlayerHealth health)
    {
        playerController = controller;
        playerHealth = health;
    }

    public void ApplySpeedBoost(float duration,float amount)
    {
        playerController.StartSpeedBoost(duration , amount);
    }

    public void ApplyDamage(float damage)
    {
        playerHealth.Damage(damage);
    }
}
