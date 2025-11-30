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

    public void TriggerEnemyHit(float damage)
    {
        onEnemyHit?.Invoke(damage);
    }
}
