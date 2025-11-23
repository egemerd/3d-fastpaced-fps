using UnityEngine;

public class SlidingState : IState
{
    public string StateName => "Sliding";

    public void EnterState(PlayerController player )
    {
        if (player.MinSlideSpeedReached() && player.isSprinting)
        {
            player.StartSlide();
        }
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exiting Sliding State");

        if(player.isSliding)
        {
            player.StopSlide();
        }   
    }

    public void UpdateState(PlayerController player, Climbing climbing)
    {
        Debug.Log("Updating Sliding State");

        player.CalculateMoveDirection();
        player.HandleSliding();
        player.Movement();
         
        if (player.GetJumpAction().triggered && player.isGrounded)
        {
            player.Jump(); 
            player.ChangeState(new JumpingState());         
        }     
        if (player.slideEnded)
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
        if (!player.isSliding)
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
