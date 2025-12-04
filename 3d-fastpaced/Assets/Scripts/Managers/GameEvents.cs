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
        onDoorClosed?.Invoke();
    }
}
