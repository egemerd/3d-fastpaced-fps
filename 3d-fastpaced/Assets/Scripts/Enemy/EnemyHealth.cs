using UnityEngine;
using System.Collections;
using static UnityEngine.ParticleSystem;

public class EnemyHealth : MonoBehaviour , IDamageable
{
    [SerializeField] private EnemyDataSO enemyData;
    public float currentHealth;
    bool isEnemyDied =false;

    private bool isPlayingHurtParticle = false;
    private float hurtParticleCooldown = 0.1f;

    private WeaponSwitching weaponSwitching;

    private void Awake()
    {
        currentHealth = enemyData.health;
        weaponSwitching = FindObjectOfType<WeaponSwitching>();
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

    private void EnemyParticle(string particleName, float minimumDuration = -1f)
    {
        GameObject particle = ObjectPoolManager.Instance.GetPooledObject(particleName);
        if (particle != null)
        {
            Debug.Log($"Playing particle: {particleName}");
            particle.transform.position = transform.position;
            particle.transform.rotation = Quaternion.identity;

            ParticleSystem ps = particle.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                // Önce stop - temiz baþlangýç
                ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
                ps.Play();

                // Partikül süresini hesapla
                float particleDuration = ps.main.duration;

                // Eðer minimum süre belirtilmiþse ve partikül süresi bundan kýsaysa, minimum süreyi kullan
                if (minimumDuration > 0 && particleDuration < minimumDuration)
                {
                    particleDuration = minimumDuration;
                }

                ObjectPoolManager.Instance.ReturnToPoolAfterDelay(particle, particleDuration);
            }
        }
        else
        {
            Debug.LogWarning($"[EnemyHealth] Could not get particle from pool: {particleName}");
        }
    }

    private void Die()
    {
        if (isEnemyDied) return;
        isEnemyDied = true;

        Debug.Log("[EnemyHealth] Die() called - Starting death sequence");
        if (weaponSwitching.selectedWeapon == 0)
        {
            EnemyParticle("EnemyDeathParticle");
        }
        else if (weaponSwitching.selectedWeapon == 1)
        {
            EnemyParticle("EnemyDeathParticlePistol");
        }
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
    private IEnumerator PlayHurtParticleWithCooldown()
    {
        isPlayingHurtParticle = true;

        // Partikülü oynat - minimum 0.5 saniye görünsün
        EnemyParticle("EnemyHurtParticle", 0.5f);

        // Cooldown süresi kadar bekle
        yield return new WaitForSeconds(hurtParticleCooldown);

        isPlayingHurtParticle = false;
    }
    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (!isPlayingHurtParticle)
        {
            StartCoroutine(PlayHurtParticleWithCooldown());
        }
        //Debug.Log($"{gameObject.name} HP: {currentHealth}");

        //if (currentHealth <= 0)
        //{
        //    Die();
        //}
    }

    
}
