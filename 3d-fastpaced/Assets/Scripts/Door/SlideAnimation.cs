using UnityEngine;
using System.Collections;
using DG.Tweening;
public class SlideAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] SlidingCameraSO slidingCamera;
    [SerializeField] SlidingCameraSO slidingDownCamera;

    [Header("Settings")]
    [SerializeField] float firstStopDuration;
    [SerializeField] float firstStopScale;
    [SerializeField] float waitAfterFirst;

    [SerializeField] float secondStopDuration;
    [SerializeField] float secondStopScale;
    [SerializeField] float waitAfterSecond;

    [SerializeField] float thirdStopDuration;
    [SerializeField] float followSpeed = 5f;

    [Header("Randomness")]
    [SerializeField] float waitBetweenCameraChange;

    Camera camera1; //instantiated camera
    Camera camera2; //instantiated camera

    SlidingCameraSO currentCameraSO; //current camera scriptable object
    Coroutine slidingCoroutine;

    private Transform player;
    private float whichCam;
    bool isSliding = false;
    float timer = 0f;

    private void Start()
    {
    }
    private void Update()
    {
        if (!isSliding) return;
        FollowPlayerCamera(currentCameraSO);
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

    
    IEnumerator StartSlidingAnimation(Transform spawnTransform)
    {
        player = spawnTransform;
        isSliding = true; // YENÝ
        timer = 0f;
        
        camera1 = Instantiate(slidingCamera.slidingCamera, spawnTransform.position + slidingCamera.CameraOffset, Quaternion.Euler(slidingCamera.CameraRotation));
        camera2 = Instantiate(slidingDownCamera.slidingCamera, spawnTransform.position + slidingDownCamera.CameraOffset, Quaternion.Euler(slidingDownCamera.CameraRotation));
        camera1.depth = 10;
        currentCameraSO = slidingCamera;
        
        while(timer < firstStopDuration)
        {
            timer += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(1f, firstStopScale, timer / firstStopDuration);
            yield return null;
        }
        timer = 0f;
        yield return new WaitForSecondsRealtime(waitAfterFirst);

        while (timer < secondStopDuration)
        {
            timer += Time.unscaledDeltaTime;

            Time.timeScale = Mathf.Lerp(firstStopScale, secondStopScale, timer / secondStopDuration);   
            yield return null;
        }
        timer = 0f;
    }



    void StopSlidingAnimation()
    {
        isSliding = false;
        StopCoroutine(slidingCoroutine);
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
            InputManager.Instance.LockInput();
            slidingCoroutine = StartCoroutine(StartSlidingAnimation(other.transform));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            InputManager.Instance.UnlockInput();
            StopSlidingAnimation();
        }
    }
}
