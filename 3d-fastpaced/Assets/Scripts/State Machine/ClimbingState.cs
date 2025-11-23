using UnityEngine;

public class ClimbingState : IState
{
    public string StateName => "Climbing";

    public void EnterState(PlayerController player)
    {
        Debug.Log("Entered Climbing State");
        Climbing climbing = player.GetComponent<Climbing>();
        if (climbing != null)
        {
            climbing.StartClimbing();
        }
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exited Climbing State");
        Climbing climbing = player.GetComponent<Climbing>();
        if (climbing != null)
        {
            climbing.StopClimbing();
        }
    }

    public void UpdateState(PlayerController player, Climbing climbing)
    {
        Debug.Log("Updating Climbing State");
        climbing.ClimbingMovement();

        if (!climbing.isClimbing || !climbing.isWallHit)
        {
            if (!player.isGrounded)
            {
                player.ChangeState(new JumpingState());
            }
            else 
            {
                if (player.isMoving)
                {
                    player.ChangeState(new WalkingState());
                }
                else
                {
                    player.ChangeState(new IdleState());
                }
            }
            
        }
    }
}
