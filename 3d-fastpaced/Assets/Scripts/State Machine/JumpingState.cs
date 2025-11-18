using UnityEngine;

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

    public void UpdateState(PlayerController player)
    {
        player.CalculateMoveDirection();
        player.Movement();

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

    }
}
