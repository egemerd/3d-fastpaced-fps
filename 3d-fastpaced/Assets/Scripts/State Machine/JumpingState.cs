using UnityEngine;
using UnityEngine.Windows;

public class JumpingState : IState
{
    string IState.StateName => "Jumping";
    public void EnterState(PlayerController player)
    {
        Debug.Log("Entered Jumping State");
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exited Jumping State");
    }

    public void UpdateState(PlayerController player, Climbing climbing)
    {
        player.CalculateMoveDirection();
        player.Movement();
        InputManager input = InputManager.Instance;
        LedgeClimbing ledgeClimb = player.GetComponent<LedgeClimbing>();
        if (ledgeClimb != null &&
            ledgeClimb.isLedgeDetected &&
         
            !player.isGrounded)
        {
            player.ChangeState(new LedgeClimbingState());
        }
            if (player.isGrounded && player.GetVelocityY()<0)
        {
            if (player.GetCrouchAction().ReadValue<float>() > 0 && player.canSlide)
            {
                player.ChangeState(new SlidingState());
            }
            if (player.isMoving)
            {
                player.ChangeState(new WalkingState());
            }
            else
            {
                player.ChangeState(new IdleState());
            }
        }
        if (climbing.isWallHit && !player.isGrounded && climbing.IsMovingForward())
        {
            player.ChangeState(new ClimbingState());
        }

    }
}
