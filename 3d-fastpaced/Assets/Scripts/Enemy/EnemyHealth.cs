using UnityEngine;
using System.Collections;
using static UnityEngine.ParticleSystem;

public class EnemyHealth : MonoBehaviour , IDamageable,IBurnable
{
    [SerializeField] private EnemyDataSO enemyData;
    [SerializeField] private WeaponStateSO weaponStateSO;
    public float currentHealth;
    bool isEnemyDied =false;


    private bool isPlayingHurtParticle = false;
    private float hurtParticleCooldown = 0.1f;

    private WeaponSwitching weaponSwitching;

    [SerializeField]
    private bool _IsBurning;
    public bool IsBurning { get => _IsBurning; set => _IsBurning = value; }

    private Coroutine BurnCoroutine;

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
        if (weaponStateSO.selectedGunIndex == 0)
        {
            EnemyParticle("EnemyDeathParticle");
            Debug.Log("[EnemyHealth] EnemyDeathParticle played for selectedGunIndex 0");
        }
        else if (weaponStateSO.selectedGunIndex == 1)
        {
            EnemyParticle("EnemyDeathParticlePistol");
            Debug.Log("[EnemyHealth] EnemyDeathParticlePistol played for selectedGunIndex 1");
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
        AudioManager.Instance.PlaySFX("EnemyDamage", 0.6f);
        currentHealth -= damage;
        if (!isPlayingHurtParticle)
        {
            StartCoroutine(PlayHurtParticleWithCooldown());
        }
        
    }

    public void BurningTakeDamage(float damage)
    {
        currentHealth -= damage;
    }
    public void StartBurning(float damagePerSecond)
    {
        IsBurning = true;
        if (BurnCoroutine != null)
        {
            StopCoroutine(BurnCoroutine);
        }

        BurnCoroutine = StartCoroutine(Burn(damagePerSecond));
    }

    public void StopBurning()
    {
        IsBurning = false;
        if (BurnCoroutine != null)
        {
            StopCoroutine(BurnCoroutine);
        }
    }



    private IEnumerator Burn(float damagePerSecond)
    {
        WaitForSeconds wait = new WaitForSeconds(0.3f);

        while (IsBurning)
        {
            TakeDamage(damagePerSecond);
            yield return wait;
        }
    }

    
}
