using UnityEngine;

[CreateAssetMenu(fileName = "SlidingCameraSO", menuName = "SlidingCameraSO", order = 1)]
public class SlidingCameraSO : ScriptableObject
{
    public Camera slidingCamera;

    [Header("Settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector3 cameraRotation;


    [Header("Randomness")]
    [SerializeField] private float camWeight = 0.8f;
    
    public float CamWeight => camWeight;
    public Vector3 CameraOffset => cameraOffset;

    public Vector3 CameraRotation => cameraRotation;
}
