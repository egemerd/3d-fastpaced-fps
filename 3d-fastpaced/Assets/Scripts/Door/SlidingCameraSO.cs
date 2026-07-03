using UnityEngine;

[CreateAssetMenu(fileName = "SlidingCameraSO", menuName = "SlidingCameraSO", order = 1)]
public class SlidingCameraSO : ScriptableObject
{
    public Camera slidingCamera;

    [Header("Settings")]
    [SerializeField] private Vector3 cameraOffset;
    [SerializeField] private Vector3 cameraRotation;


    public Vector3 CameraOffset => cameraOffset;

    public Vector3 CameraRotation => cameraRotation;
}
