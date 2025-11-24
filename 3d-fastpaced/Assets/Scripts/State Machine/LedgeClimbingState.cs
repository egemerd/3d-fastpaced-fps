using UnityEngine;

public class LedgeClimbingState : IState
{
    public string StateName => "LedgeClimbing";

    public void EnterState(PlayerController player)
    {
        Debug.Log("Entered Ledge Climbing State");
        LedgeClimbing ledge = player.GetComponent<LedgeClimbing>();

        if (ledge!= null)
        {
            ledge.StartLedgeClimb(player);
        }
    }

    public void ExitState(PlayerController player)
    {
        Debug.Log("Exited Ledge Climbing State");
        LedgeClimbing ledge = player.GetComponent<LedgeClimbing>();

        if (ledge != null)
        {
            ledge.StopLedgeClimb(player);
        }

        CharacterController cc = player.GetComponent<CharacterController>();
        if (cc != null && !cc.enabled)
        {
            Debug.LogWarning("CharacterController was disabled! Re-enabling...");
            cc.enabled = true;
        }

    }

    public void UpdateState(PlayerController player, Climbing climbing)
    {
        Debug.Log("Updating Ledge Climbing State");

        LedgeClimbing ledgeClimb = player.GetComponent<LedgeClimbing>();

        if (ledgeClimb == null)
        {
            Debug.LogError("LedgeClimbing component not found!");
            player.ChangeState(new IdleState());
            
        }


        if (!ledgeClimb.isLedgeClimbing)
        {
            // Zeminde mi kontrol et
            if (player.isGrounded)
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
            else
            {
                //  YENÝ: Hala havadaysa JumpingState'e dön
                Debug.LogWarning("Ledge climb ended but player is airborne!");
                player.ChangeState(new JumpingState());
            }
            
        }

    }
}
