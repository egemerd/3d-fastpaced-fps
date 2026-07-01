using UnityEngine;

public class SlideAnimation : MonoBehaviour
{
    [SerializeField] Camera slidingCamera;
    [SerializeField] Vector3 cameraOffset;
    [SerializeField] Vector3 cameraRotation;
    [SerializeField] float timeScaleForSliding;
    Camera camera;

    void StartSlidingAnimation(Transform spawnTransform)
    {
        camera = Instantiate(slidingCamera, spawnTransform.position + cameraOffset, Quaternion.Euler(cameraRotation));
        Time.timeScale = timeScaleForSliding;
        camera.depth = 10;
    }

    void StopSlidingAnimation()
    {
        camera.depth = -10;
        Time.timeScale = 1f;
        Destroy(camera.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StartSlidingAnimation(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            StopSlidingAnimation();
        }
    }
}
