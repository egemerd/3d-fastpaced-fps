using UnityEngine;

public class SlideAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SlidingCameraSO slidingCamera;
    [SerializeField] SlidingCameraSO slidingDownCamera;

    [Header("Settings")]
    [SerializeField] float timeScaleForSliding;
    [SerializeField] float followSpeed = 5f;

    [Header("Randomness")]
    [SerializeField] float waitBetweenCameraChange;

    Camera camera1; //instantiated camera
    Camera camera2; //instantiated camera

    SlidingCameraSO currentCameraSO; //current camera scriptable object

    private Transform player;
    private float whichCam;
    bool isSliding = false;
    float timer = 0f;
    float cam1Weight;
    float cam2Weight;

    private void Start()
    {
        cam1Weight = slidingCamera.CamWeight;
        cam2Weight = slidingDownCamera.CamWeight;
    }
    private void Update()
    {
        if (!isSliding) return;
        RandomCamera();
        FollowPlayerCamera(currentCameraSO);
    }

    private void RandomCamera()
    {
        

        if (timer >= waitBetweenCameraChange)
        {
            float weight = cam1Weight + cam2Weight;
            float random = Random.Range(0f, weight);
            if (random < cam1Weight)
            {
                currentCameraSO = slidingCamera;
                camera1.depth = 10;
                camera2.depth = -10;
            }
            else
            {
                currentCameraSO = slidingDownCamera;
                camera1.depth = -10;
                camera2.depth = 10;
            }
            timer = 0f;
        }
        else
        {
            timer += Time.unscaledDeltaTime;
        }
        
    }

    void FollowPlayerCamera(SlidingCameraSO cam)
    {
        Camera activeCam = (cam == slidingCamera) ? camera1 : camera2;
        Vector3 targetPosition = activeCam.transform.position;
        targetPosition.x = player.position.x + cam.CameraOffset.x; // sadece X ekseni takip

        activeCam.transform.position = Vector3.Lerp(
            activeCam.transform.position,
            targetPosition,
            Time.unscaledDeltaTime * followSpeed // unscaledDeltaTime çünkü Time.timeScale deðiþiyor!
        );
    }

    
    void StartSlidingAnimation(Transform spawnTransform)
    {
        player = spawnTransform;
        isSliding = true; // YENÝ
        timer = 0f;

        camera1 = Instantiate(slidingCamera.slidingCamera, spawnTransform.position + slidingCamera.CameraOffset, Quaternion.Euler(slidingCamera.CameraRotation));
        camera2 = Instantiate(slidingDownCamera.slidingCamera, spawnTransform.position + slidingDownCamera.CameraOffset, Quaternion.Euler(slidingDownCamera.CameraRotation));

        currentCameraSO = slidingCamera;
        Time.timeScale = timeScaleForSliding;

        camera1.depth = 10;
        camera2.depth = -10;
    }

    void StopSlidingAnimation()
    {
        isSliding = false;

        Destroy(camera1.gameObject);
        Destroy(camera2.gameObject);
        camera1 = null;
        camera2 = null;

        Time.timeScale = 1f;
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
