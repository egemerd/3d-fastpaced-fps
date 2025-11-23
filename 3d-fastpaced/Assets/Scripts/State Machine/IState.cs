using UnityEngine;

public interface IState 
{
    string StateName { get; }
    public void EnterState(PlayerController player);   
    public void UpdateState(PlayerController player, Climbing climbing);   
    public void ExitState(PlayerController player);   
}
