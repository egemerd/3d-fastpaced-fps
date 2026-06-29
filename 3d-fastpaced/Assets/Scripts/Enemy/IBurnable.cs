using UnityEngine;

public interface IBurnable
{
    public bool IsBurning { get; set; }
    void StartBurning(float damagePerSecond);

    void StopBurning();
    
}
