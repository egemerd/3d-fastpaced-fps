using System.Collections;
using UnityEngine;

public class EnemyChaser : EnemyAI
{
    private bool canAtack = true;
    private ParticleSystem attackEffect;


    private void Start()
    {
        SetMovementSpeed();
        attackEffect = GetComponentInChildren<ParticleSystem>();
        if (attackEffect == null)
        {
            Debug.LogError("[EnemyChaser] attackEffect ParticleSystem is not assigned!");
        }
    }
    protected override void EnemyAttack()
    {
        ChaserAttack();
    }

    protected override void ChasePlayer()
    {
        base.agent.SetDestination(player.position);
    }

    private void ChaserAttack()
    {
        if (!canAtack) return;
        Debug.Log("Chaser Attacking Player");
        attackEffect.Play();
        GameEvents.current.TriggerPlayerHit(damage);
        StartCoroutine(AttackCooldown());
    }

    private IEnumerator AttackCooldown()
    {
        canAtack = false;
        yield return new WaitForSeconds(timeBetweenShot);
        canAtack = true;
    }

    private void Dash()
    {

    }
}
