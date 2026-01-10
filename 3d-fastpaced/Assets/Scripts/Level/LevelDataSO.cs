using UnityEngine;
[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelDataSO", order = 1)]
public class LevelDataSO : ScriptableObject
{
    public float sliderSpeed;
    public float sliperOpenAmount;
    public Color wallColor;
    public Color floorColor;

}
