using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    [Header("References")]
    Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private PlayerInput playerInput;
    private Camera mainCamera;
    
    [Header("Input")]
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 2f;

    [Header("Look")]
    [SerializeField] private float mouseSens;
    [SerializeField] private float lookRange;


    private bool isMoving;
    Vector3 moveDirection;
    private void Start()
    {
        mainCamera = Camera.main;
        moveAction = playerInput.actions.FindAction("Move");
        jumpAction = playerInput.actions.FindAction("Jump");
        sprintAction = playerInput.actions.FindAction("Sprint");
        lookAction = playerInput.actions.FindAction("Look");
        
        Cursor.lockState= CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
        
    }
    private void Update()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        PlayerLook();
        
    }

    private void FixedUpdate()
    {
        Movement();  
    }

    private void Movement()
    {
        float speedtMultiplier = sprintAction.ReadValue<float>() > 0 ? sprintMultiplier : 1f;

        CalculateMoveDirection();

        Vector3 newVelocity = moveDirection * moveSpeed ;
        newVelocity.y = rb.linearVelocity.y; 
        rb.linearVelocity = newVelocity;

    }

    private void CalculateMoveDirection()
    {
        Vector3 forward = mainCamera.transform.forward;
        Vector3 right = mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void PlayerLook()
    {
        transform.Rotate(Vector3.up * lookInput.x * mouseSens * Time.deltaTime) ;

        verticalRotation -= lookInput.y* mouseSens *Time.deltaTime;
        verticalRotation = Mathf.Clamp(verticalRotation, -lookRange, lookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    public void StartSpeedBoost(float duration , float amount)
    {
        StartCoroutine(SpeedBoost(duration, amount));
        Debug.Log("New Speed: " + moveSpeed);
    }
    private IEnumerator SpeedBoost(float duration, float amount)
    {
        float baseSpeed = moveSpeed;
        moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseSpeed;
    }
    

}
