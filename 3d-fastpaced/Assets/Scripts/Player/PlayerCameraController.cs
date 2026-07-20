using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    private PlayerInputActions inputActions;
    Camera mainCamera;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float verticalLookLimit = 90f;

    [Header("Camera Tilt")]
    [SerializeField] private float tiltAngle = 5f; // Kameranýn yatma açýsý
    [SerializeField] private float tiltSpeed = 10f;

    [SerializeField] private bool isCameraLocked = false;
    private float currentTilt = 0f;
    private float xRotation = 0f; 
    private float yRotation = 0f; 


    private Vector2 lookInput;


    private void Awake()
    {
        inputActions = new PlayerInputActions();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        

        yRotation = transform.eulerAngles.y;
        xRotation = cameraHolder.localEulerAngles.x;

        if (xRotation > 180f)
            xRotation -= 360f;
    }

    private void Update()
    {
        LockCamera();
        if (InputManager.Instance.IsInputLocked) return;
        if (isCameraLocked) return;
        lookInput = inputActions.Gameplay.Look.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        if(PauseMenu.isPaused)
            return;
        RotateCharacter();
        LeftRightMovement();
    }

    private void LockCamera()
    {
        bool input =InputManager.Instance.lockCameraAction.WasPressedThisFrame();
        if (input)
        {
            isCameraLocked = !isCameraLocked;
        }
    }
    private void OnEnable()
    {
        inputActions.Gameplay.Enable();
    }
    private void OnDisable()
    {
        inputActions.Gameplay.Disable();
    }

    private void RotateCharacter()
    {
        float mouseX = lookInput.x * mouseSensitivity ;    
        float mouseY = lookInput.y * mouseSensitivity ;

        yRotation += mouseX;

        xRotation -= mouseY; 
        xRotation = Mathf.Clamp(xRotation, -verticalLookLimit, verticalLookLimit);

        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    private void LeftRightMovement()
    {
        float targetTilt = 0f;

        // A tuþuna basýldýðýnda (sol hareket) kamera sola yatar
        if (InputManager.Instance.moveInput.x < -0.01f)
        {
            targetTilt = -tiltAngle;
        }
        // D tuþuna basýldýðýnda (sað hareket) kamera saða yatar
        else if (InputManager.Instance.moveInput.x > 0.01f)
        {
            targetTilt = tiltAngle;
        }

        // Yumuþak geçiþ için Lerp kullan
        currentTilt = Mathf.Lerp(currentTilt, targetTilt, Time.deltaTime * tiltSpeed);

        // Kamerayý Z ekseninde döndür (roll/tilt efekti)
        mainCamera.transform.localRotation = Quaternion.Euler(
            mainCamera.transform.localEulerAngles.x,  // Bu deðer tutarsýz olabilir
            0f,
            currentTilt
        );
    }
}
