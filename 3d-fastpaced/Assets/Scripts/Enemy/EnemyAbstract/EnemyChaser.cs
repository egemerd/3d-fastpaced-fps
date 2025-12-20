using UnityEngine;

public class EnemyChaser : EnemyAI
{
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

    }

    private void Dash()
    {

    }
}
