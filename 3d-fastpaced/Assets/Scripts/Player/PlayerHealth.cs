using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float health = 100;

    private void Start()
    {
        GameEvents.current.onProjectileHitsEnemy += Damage;
    }

    public void Damage(float damage)
    {
        health = Mathf.Clamp(health-damage,0,100);
        Debug.Log("Taken damage. New Health: " + health);
    }
}
