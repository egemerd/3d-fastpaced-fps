using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class MilkGunAttackArea : MonoBehaviour
{
    public event Action<EnemyAI> EnemyEnteredEvent;
    public event Action<EnemyAI> EnemyExitedEvent;

    private List<EnemyAI> EnemiesInRadius = new List<EnemyAI>();

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Enemy entered the attack area.");

        if (other.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            Debug.Log($"Enemy {enemy.name} entered the attack area.");
            EnemiesInRadius.Add(enemy);
            EnemyEnteredEvent?.Invoke(enemy);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent<EnemyAI>(out EnemyAI enemy))
        {
            Debug.Log($"Enemy {enemy.name} exited the attack area.");
            EnemiesInRadius.Add(enemy);
            EnemyExitedEvent?.Invoke(enemy);
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in EnemiesInRadius)
        {
            EnemyExitedEvent?.Invoke(enemy);
        }
    }
}
