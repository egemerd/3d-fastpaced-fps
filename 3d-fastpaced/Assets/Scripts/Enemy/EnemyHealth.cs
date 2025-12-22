using UnityEngine;

public class EnemyHealth : MonoBehaviour , IDamageable
{
    [SerializeField] private EnemyDataSO enemyData;
    public float currentHealth;
    bool isEnemyDied =false;

    private void Awake()
    {
        currentHealth = enemyData.health;
    }

    private void Update()
    {
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnEnable()
    {
        if (GameEvents.current != null)
        {
            Debug.Log("EnemyHealth subscribed to onEnemyHit");
            GameEvents.current.onEnemyHit += TakeDamage;
        }
    }

    private void OnDisable()
    {
        if (GameEvents.current != null)
        {
            Debug.Log("EnemyHealth unsubscribed from onEnemyHit");
            GameEvents.current.onEnemyHit -= TakeDamage;
        }
    }


    private void Die()
    {
        if(isEnemyDied) return; 
        GameEvents.current.TriggerEnemyDeath();
        Destroy(gameObject, 0.1f);
        Debug.Log("Enemy Died");

        isEnemyDied = true;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        Debug.Log($"{gameObject.name} HP: {currentHealth}");

        //if (currentHealth <= 0)
        //{
        //    Die();
        //}
    }

}
