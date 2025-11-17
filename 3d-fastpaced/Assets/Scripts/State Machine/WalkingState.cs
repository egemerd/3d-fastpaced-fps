using UnityEngine;

public class WalkingState : IState
{
    string IState.StateName => "Walking";

    public void EnterState(PlayerController player)
    {
        Debug.Log("Entered Walking State");
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exited Walking State");
    }

    public void UpdateState(PlayerController player)
    {
        Debug.Log("Updating Walking State");

        player.CalculateMoveDirection();
        player.Movement();

        if (player.jumpAction.triggered && player.isGrounded)
        {
            player.Jump();
            player.ChangeState(new JumpingState());
        }
        if (player.crouchAction.ReadValue<float>() > 0 && !player.isSliding && player.canSlide && player.isGrounded)
        {
            player.ChangeState(new SlidingState());
        }
        if (!player.isMoving && player.isGrounded)
        {
            player.ChangeState(new IdleState());
        }
        

    }
}
