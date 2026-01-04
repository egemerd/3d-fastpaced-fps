using UnityEngine;

public class Boost : MonoBehaviour , ICollectibles
{
    [SerializeField] float effectDuration = 1f;
    [SerializeField] float effectAmount = 10f;

    public void ApplyEffect()
    {
        CollectibleManager.Instance.ApplySpeedBoost(effectDuration, effectAmount);
        PlayerEffects.Instance.ColorHueShift(effectDuration);
        Debug.Log("Boost");
    }

    
}
