using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100;

    private void Start()
    {
        GameEvents.current.onProjectileHitsEnemy += Damage;
        GameEvents.current.onPlayerHit += Damage;
    }

    public void Damage(float damage)
    {
        health = Mathf.Clamp(health-damage,0,100);
        PlayerDie();
        Debug.Log("Taken damage. New Health: " + health);
    }

    private void PlayerDie()
    {
        if (health <= 0)
        {
            Debug.Log("Player Died");
            GameEvents.current.TriggerPlayerDeath();
        }
    }
}
