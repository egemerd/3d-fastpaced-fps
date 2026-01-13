using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        // Singleton güvenliði
        if (current != null && current != this)
        {
            Destroy(gameObject);
            return;
        }
        current = this;
        DontDestroyOnLoad(gameObject);
    }

    public event Action<float> onEnemyHit;
    public event Action onPlayerDeath;
    public event Action onEnemyDeath;
    public event Action onDoorClosed;
    public event Action<float> onProjectileHitsEnemy;
    public event Action<float> onPlayerHit;
    

    public void TriggerProjectileHitsEnemy(float enemyDamage)
    {
        Debug.Log("TriggerEnemyHit called with damage: " + enemyDamage);
        onProjectileHitsEnemy?.Invoke(enemyDamage);
    }
    public void TriggerPlayerHit(float damage)
    {
        Debug.Log("TriggerPlayerHit called with damage: " + damage);
        onPlayerHit?.Invoke(damage);
    }
    public void TriggerEnemyHit(float damage)
    {
        onEnemyHit?.Invoke(damage);      
    }
    public void TriggerPlayerDeath()
    {
        onPlayerDeath?.Invoke();
    }
    public void TriggerEnemyDeath()
    {
        Debug.Log("enemy death trigger");
        onEnemyDeath?.Invoke();
    }
    public void TriggerDoorClosed()
    {
        Debug.Log("[Game Events] trigger door closed.");
        onDoorClosed?.Invoke();
    }
}
