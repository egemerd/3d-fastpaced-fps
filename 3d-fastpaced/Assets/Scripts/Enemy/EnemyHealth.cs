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
            isEnemyDied = false;
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

    private void EnemyDeathParticle()
    {
        Debug.Log("enemy death particle");
        GameObject deathParticle = ObjectPoolManager.Instance.GetPooledObject("EnemyDeathParticle");
        if (deathParticle != null)
        {
            deathParticle.transform.position = transform.position;
            deathParticle.transform.rotation = Quaternion.identity;
            ParticleSystem ps = deathParticle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                ObjectPoolManager.Instance.ReturnToPoolAfterDelay(deathParticle, ps.main.duration);
            }
        }
    }

    private void Die()
    {
        if (isEnemyDied) return;
        isEnemyDied = true;

        Debug.Log("[EnemyHealth] Die() called - Starting death sequence");

        EnemyDeathParticle();
        TimeStopEffect.Instance.TimeStopPreset("EnemyDeath");
        if (GameEvents.current != null)
        {
            Debug.Log("[EnemyHealth] Calling TriggerEnemyDeath()...");
            GameEvents.current.TriggerEnemyDeath();
            Debug.Log("[EnemyHealth] TriggerEnemyDeath() completed");
        }
        else
        {
            Debug.LogError("[EnemyHealth] GameEvents.current is NULL! Cannot trigger death event!");
        }

       
        Destroy(gameObject, 0.15f); 
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        //Debug.Log($"{gameObject.name} HP: {currentHealth}");

        //if (currentHealth <= 0)
        //{
        //    Die();
        //}
    }

}
