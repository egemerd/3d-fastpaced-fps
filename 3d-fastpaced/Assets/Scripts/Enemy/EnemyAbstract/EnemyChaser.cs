using System.Collections;
using UnityEngine;


public class EnemyChaser : EnemyAI
{
    private bool canAtack = true;
    private ParticleSystem attackEffect;
    [SerializeField] private float firstTouchAttackCooldown = 1f;


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
        if (canAtack)
        {
            StartCoroutine(ChaderAttackCoroutine());
        }
        //ChaserAttack();
    }

    protected override void ChasePlayer()
    {
        base.agent.SetDestination(player.position);
    }
    private IEnumerator ChaderAttackCoroutine()
    {
        canAtack = false;
        yield return new WaitForSeconds(firstTouchAttackCooldown);
        Debug.Log("Chaser Attacking Player");
        attackEffect.Play();
        GameEvents.current.TriggerPlayerHit(damage);
        yield return new WaitForSeconds(timeBetweenShot);
        canAtack = true;
    }
    private void ChaserAttack()
    {
        if (!canAtack) return;
        StartCoroutine(AttackCooldown(firstTouchAttackCooldown));
        Debug.Log("Chaser Attacking Player");
        attackEffect.Play();
        GameEvents.current.TriggerPlayerHit(damage);
        StartCoroutine(AttackCooldown(timeBetweenShot));
    }

    private IEnumerator AttackCooldown(float duration)
    {
        canAtack = false;
        yield return new WaitForSeconds(duration);
        canAtack = true;
    }



    private void Dash()
    {

    }
}
