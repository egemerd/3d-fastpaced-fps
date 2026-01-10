using UnityEngine;
using UnityEngine.AI;

public class EnemyShooter : EnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed;

    bool isReadyToShoot = false;
    
    public Animator myAnimator;
    private Vector3 cachedSpawnPos;
    private Vector3 cachedDirection;

    
    private void Start()
    {
        projectilePrefab.GetComponent<EnemyProjectile>().SetSpeed(projectileSpeed);
        myAnimator= GetComponentInChildren<Animator>();
    }

    
    protected override void EnemyAttack()
    {
        Debug.Log("[EnemyShooter] EnemyAttack called.");
        EnemyShoot();
    }
    protected override void ChasePlayer()
    {
        base.agent.SetDestination(transform.position); // Düþman hareket etmesin
    }
    private void EnemyShoot()
    {  
        if (Time.time < nextShootTime) return;

        Vector3 targetPos = player.position;
        cachedSpawnPos = transform.position + transform.up * 1.5f + transform.forward * 0.5f;  // düþmanýn önünden spawn
        cachedDirection = (targetPos - cachedSpawnPos).normalized;
        
        isReadyToShoot = true;
        
        myAnimator.SetTrigger("Throwing");

        // Cooldown ayarla
        nextShootTime = Time.time + timeBetweenShot;
        
    }

    void InstantiateProjectile()
    {
        // Atýþ anýndaki player pozisyonunu yakala
        Quaternion projectileRot = Quaternion.LookRotation(cachedDirection);
        GameObject projectile = Instantiate(projectilePrefab, cachedSpawnPos, projectileRot);

        var enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null)
        {
            enemyProjectile.SetDirection(cachedDirection);
            enemyProjectile.SetDamage(damage);
        }
        else
        {
            Debug.LogError("[EnemyShooter] EnemyProjectile component yok!");
        }
    }

    public void OnThrowProjectile()
    {
        if (!isReadyToShoot) { return; } 
        
        
        InstantiateProjectile();
        
        isReadyToShoot = false;

    }

    
}
