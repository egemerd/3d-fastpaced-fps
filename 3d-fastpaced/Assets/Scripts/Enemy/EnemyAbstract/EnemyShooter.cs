using UnityEngine;

public class EnemyShooter : EnemyAI
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float timeBetweenShot;

    private float nextShootTime = 0f;

    
    protected override void EnemyAttack()
    {
        EnemyShoot();
    }
    protected override void ChasePlayer()
    {
        base.agent.SetDestination(transform.position); // Düþman hareket etmesin
    }
    private void EnemyShoot()
    {
        if (Time.time < nextShootTime) return;

        // Atýþ anýndaki player pozisyonunu yakala
        Vector3 targetPos = player.position;

        // Direction'ý spawn noktasýndan hedefe hesapla
        Vector3 spawnPos = transform.position + transform.forward * 0.5f; // düþmanýn önünden spawn
        Vector3 dir = (targetPos - spawnPos).normalized;

        // Projectile rotasyonunu direction'a göre ayarla
        Quaternion projectileRot = Quaternion.LookRotation(dir);

        // Instantiate
        GameObject projectile = Instantiate(projectilePrefab, spawnPos, projectileRot);

        // Direction'ý hemen set et (Update çaðrýlmadan önce)
        var enemyProjectile = projectile.GetComponent<EnemyProjectile>();
        if (enemyProjectile != null)
        {
            enemyProjectile.SetDirection(dir);
            enemyProjectile.SetDamage(damage);
        }
        else
        {
            Debug.LogError("[EnemyShooter] EnemyProjectile component not found on projectile instance.");
        }

        // Cooldown ayarla
        nextShootTime = Time.time + timeBetweenShot;
    }
}
