using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "GameSettingsSO", order = 1)]
public class GameSettingsSO : ScriptableObject
{
    [Header("Game Movement Settings")]
    [SerializeField] private bool sprintToggleMode;
    public bool SprintToggleMode => sprintToggleMode;
}
