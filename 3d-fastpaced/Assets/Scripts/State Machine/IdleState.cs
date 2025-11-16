using UnityEngine;

public class IdleState : IState
{
    string IState.StateName => "Idle";  
    public void EnterState(PlayerController player)
    {
        Debug.Log("Entered Idle State");
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exited Idle State");
    }

    public void UpdateState(PlayerController player)
    {  
        if (player.isMoving && player.isGrounded)
        {
            player.ChangeState(new WalkingState());
        }
                           
        if (player.jumpAction.triggered && player.isGrounded)
        {
            player.Jump();
            player.ChangeState(new JumpingState());
        }
    }   
}
