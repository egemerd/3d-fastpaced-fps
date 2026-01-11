using UnityEngine;

public class CameraShakeMovement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Camera weaponCamera;
    private float originalCameraRotationZ;
    [SerializeField] private float targetCameraRotationZ;

    [Header("Sway Settings")]
    [SerializeField] private float swayAmount = 1.5f;
    [SerializeField] private float swayFrequency = 2.5f;
    [SerializeField] private float swaySpeed = 1.2f;

    [Header("FOV Settings")]
    [SerializeField] private float targetFov = 82f;
    [SerializeField] private float targetFovSliding = 90f;
    [SerializeField] private float fovTransitionSpeed = 4f;
    [SerializeField] private float targetFovWeapon = 82f;
    [SerializeField] private float fovTransitionSpeedWeapon = 4f;

    private float initialFov;
    private void Start()
    {
        initialFov = playerCamera.fieldOfView;
    }
    public void SprintSway(Vector2 moveInput)
    {
        Debug.Log("Sprinting Swaying" + moveInput);
        float swayAmountZ = Mathf.Lerp(originalCameraRotationZ,targetCameraRotationZ, 
            swaySpeed * Time.deltaTime);
        if (moveInput.x ==1)
        {
            Debug.Log("Swaying Right");
            playerCamera.transform.localEulerAngles = new Vector3(
                playerCamera.transform.localEulerAngles.x,
                playerCamera.transform.localEulerAngles.y,
                swayAmountZ
            );
        }
        else if(moveInput.x == -1)
        {
            Debug.Log("Swaying Left");
            playerCamera.transform.localEulerAngles = new Vector3(
                playerCamera.transform.localEulerAngles.x,
                playerCamera.transform.localEulerAngles.y,
                -swayAmountZ
            );
        }

            
    }

    public void SprintFovBoost(bool isSprinting , bool isSliding, Vector2 moveInput)
    {
        if (isSprinting && moveInput.y > 0.01f)
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFov,
                fovTransitionSpeed * Time.deltaTime);
        }
        else if(isSliding && isSprinting)
        {
            Debug.Log(" [CameraShakeMovement] Sliding FOV Boost"); 
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFovSliding,
               fovTransitionSpeed * Time.deltaTime);
        }
        else
        {
            playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, initialFov,
                fovTransitionSpeed * Time.deltaTime);
        }
    }

    public void SprintFovBoostWeapon(bool isSprinting , bool isSliding, Vector2 moveInput)
    {
        if (isSprinting && moveInput.y > 0.01f)
        {
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, targetFov,
                fovTransitionSpeedWeapon * Time.deltaTime);
        }
        else if (isSliding)
        {
            playerCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, targetFovSliding,
               fovTransitionSpeed * Time.deltaTime);
        }
        else
        {
            weaponCamera.fieldOfView = Mathf.Lerp(weaponCamera.fieldOfView, initialFov,
                fovTransitionSpeedWeapon * Time.deltaTime);
        }
    }

}
