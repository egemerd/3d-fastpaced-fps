using UnityEngine;

public class ProjectileRotationAnim : MonoBehaviour
{
    [Header("Rotation Settings")]
    [Tooltip("X ekseninde dönme hızı")]
    [SerializeField] private float rotationSpeedX = 360f;

    [Tooltip("Y ekseninde dönme hızı")]
    [SerializeField] private float rotationSpeedY = 180f;

    [Tooltip("Z ekseninde dönme hızı")]
    [SerializeField] private float rotationSpeedZ = 0f;

    [Header("Smoothness")]
    [Tooltip("Rotasyon yumuşatma (yüksek = daha smooth)")]
    [SerializeField] private float smoothing = 5f;

    private Quaternion targetRotation;
    private Vector3 currentRotationVelocity;

    private void Start()
    {
        targetRotation = transform.rotation;
    }

    private void Update()
    {
        // Hedef rotasyonu hesapla
        Vector3 rotationDelta = new Vector3(
            rotationSpeedX * Time.deltaTime,
            rotationSpeedY * Time.deltaTime,
            rotationSpeedZ * Time.deltaTime
        );

        targetRotation *= Quaternion.Euler(rotationDelta);

        // Smooth interpolation
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            Time.deltaTime * smoothing
        );
    }
}
