using System;
using UnityEngine;

public class GameEvents : MonoBehaviour
{
    public static GameEvents current;

    private void Awake()
    {
        current = this;
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
        onEnemyDeath?.Invoke();
    }
    public void TriggerDoorClosed()
    {
        onDoorClosed?.Invoke();
    }
}
