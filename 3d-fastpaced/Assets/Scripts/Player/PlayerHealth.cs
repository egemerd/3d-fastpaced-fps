using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private TextMeshProUGUI healthText;

    private void Start()
    {
        GameEvents.current.onProjectileHitsEnemy += Damage;
        GameEvents.current.onPlayerHit += Damage;
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
}
