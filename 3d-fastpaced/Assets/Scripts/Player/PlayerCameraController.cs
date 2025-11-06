using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHolder;
    private PlayerInputActions inputActions;

    [Header("Look")]
    [SerializeField] private float mouseSensitivity = 1f;
    [SerializeField] private float verticalLookLimit = 90f;

    private float xRotation = 0f; 
    private float yRotation = 0f; 


    private Vector2 lookInput;


    private void Awake()
    {
        inputActions = new PlayerInputActions();
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
        lookInput = inputActions.Gameplay.Look.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        RotateCharacter();
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
}
