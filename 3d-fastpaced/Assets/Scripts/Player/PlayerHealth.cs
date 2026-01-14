using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private float healAmount = 10f;

    private void Start()
    {
        GameEvents.current.onProjectileHitsEnemy += Damage;
        GameEvents.current.onPlayerHit += Damage;
        GameEvents.current.onEnemyDeath += Heal;
        healthText.text = "Health: " + health.ToString("0");
    }

    public void Damage(float damage)
    {
        health = Mathf.Clamp(health-damage,0,100);
        healthText.text = "Health: " + health.ToString("0");
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

    private void Heal()
    {
        if(health < 100)
        {
            health = Mathf.Clamp(health + healAmount, 0, 100);
            healthText.text = "Health: " + health.ToString("0");
            Debug.Log("Player Healed. health amount : " + healAmount);
        }
    }
}
