using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private float lifetime = 10f;
    Vector3 projectileDirection;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }
    private void Update()
    {
        ProjectileMove(projectileDirection);
    }

    public void SetDirection(Vector3 direction)
    {
        projectileDirection = direction;
    }
    void ProjectileMove(Vector3 direction)
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
