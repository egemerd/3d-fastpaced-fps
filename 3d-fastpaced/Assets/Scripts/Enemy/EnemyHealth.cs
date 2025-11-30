using UnityEngine;

public class EnemyHealth : MonoBehaviour , IDamageable
{
    [SerializeField] private EnemyDataSO enemyData;
    public float currentHealth;

    private void Awake()
    {
        currentHealth = enemyData.health;
    }

    private void OnEnable()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyHit += TakeDamage;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.current != null)
        {
            GameEvents.current.onEnemyHit -= TakeDamage;
        }
    }


    private void Die()
    {
        Destroy(gameObject);
        Debug.Log("Enemy Died");
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} HP: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

}
