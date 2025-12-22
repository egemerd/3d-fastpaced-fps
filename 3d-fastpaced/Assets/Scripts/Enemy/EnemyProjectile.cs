using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    Vector3 projectileDirection;
    [SerializeField] float baseDamage = 10f;    
    private float damage;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void Update()
    {
        ProjectileMove(projectileDirection);
    }

    public void SetSpeed(float speedAmount)
    {
        speed = speedAmount;
    }

    public void SetDirection(Vector3 direction)
    {
        projectileDirection = direction;
    }
    void ProjectileMove(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
    }

    public void SetDamage(float damage)
    {
        Debug.Log("EnemyProjectile SetDamage called with damage: " + damage);
        baseDamage = damage;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameEvents.current.TriggerProjectileHitsEnemy(baseDamage);
            Destroy(gameObject);
        }
    }
}
