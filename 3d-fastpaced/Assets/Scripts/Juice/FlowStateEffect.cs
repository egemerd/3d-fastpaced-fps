using System.Collections;
using UnityEngine;

public class FlowStateEffect : MonoBehaviour
{
    [SerializeField] private LevelDataSO currentLevelData;






    private IEnumerator FlowStateCoroutine()
    {
        float elapsedTime = 0f;
        float duration = currentLevelData.flowStateDuration;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            // Apply the flow state effect based on the level data
            // For example, you can adjust the player's speed, visual effects, etc.
            // This is just a placeholder for your actual implementation.
            ApplyFlowStateEffect(t);
            yield return null;
        }
        // Reset or end the flow state effect after the duration
        EndFlowStateEffect();
    }
}
}
