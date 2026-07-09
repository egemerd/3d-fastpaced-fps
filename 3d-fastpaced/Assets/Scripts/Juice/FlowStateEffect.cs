using System;
using System.Collections;
using UnityEngine;

public class FlowStateEffect : MonoBehaviour
{
    [SerializeField] private LevelDataSO currentLevelData;
    [SerializeField] private Color flowColor = Color.red; 

    private void Update()
    {
        if (InputManager.Instance.crouchAction.WasPressedThisFrame())
        {
            FlowState();
        }
    }
    private void FlowState()
    {
        ParticlePaintManager.Instance.ApplyOverride(flowColor);
    }
}

