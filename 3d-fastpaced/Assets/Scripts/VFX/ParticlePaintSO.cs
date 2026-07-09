using UnityEngine;

[CreateAssetMenu(fileName = "ParticlePaintSettings", menuName = "ParticlePaintSettings")]
public class ParticlePaintSO : ScriptableObject
{
    [SerializeField] private Color paintColor;

    public Color PaintColor => paintColor;
}
