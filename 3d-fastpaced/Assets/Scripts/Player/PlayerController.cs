using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHead;
    [SerializeField] private Transform bodyTransform;
    Rigidbody rb;
    private InputAction moveAction;
    private InputAction sprintAction;
    private InputAction jumpAction;
    private InputAction lookAction;
    private PlayerInput playerInput;
    private Camera mainCamera;
    private CharacterController characterController;
    

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

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float sprintingJumpHeight = 2.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float fallGravity = 1.5f;
    [SerializeField] private float airControlTime = -9.81f;
    [SerializeField] private float jumpRayOffset = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;

    private Vector3 velocity;
    private bool isGrounded;
    private bool isMoving;
    Vector3 moveDirection;
    private bool isSprinting;   


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
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        GroundCheck();
        Debug.Log(isGrounded);
        ApplyGravity();

        if (jumpAction.triggered && isGrounded)
        {
            Jump();
        }

        CalculateMoveDirection();
        Movement();
    }


    

    private void Movement()
    {
        float speedMultiplier = sprintAction.ReadValue<float>() > 0 && isGrounded ? sprintMultiplier : 1f;
    
        Vector3 horizontalMove = moveDirection * moveSpeed * speedMultiplier * Time.deltaTime;

        
        Vector3 verticalMove = velocity * Time.deltaTime;

        Vector3 finalMove = horizontalMove + verticalMove;
        characterController.Move(finalMove);

    }

    private void GroundCheck()
    {
        Vector3 bodyPosition = bodyTransform.position;
        isGrounded = Physics.Raycast(bodyPosition , 
            Vector3.down , 
            (characterController.height/2 + jumpRayOffset),
            groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    private void Jump()
    {
        isSprinting = sprintAction.ReadValue<float>() > 0;
        float height = jumpHeight;

        if(isSprinting)
        {
            height = sprintingJumpHeight;
            
        }
        velocity.y = Mathf.Sqrt(height * -2 * gravity);
    }

    private void ApplyGravity()
    {
        float currentGravity = gravity;

        if (velocity.y < 0)
        {
            Debug.Log("Applying Fall Gravity");
            Debug.Log(velocity.y);
            currentGravity *= fallGravity;
        }
        Debug.Log("Current Gravity: " + currentGravity);
        velocity.y += currentGravity * Time.deltaTime;
    }

    private void CalculateMoveDirection()
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();
        moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void OnDrawGizmos()
    {
        if (characterController != null)
        {
            Gizmos.color = isGrounded ? Color.green : Color.red;
            Vector3 checkPosition = transform.position + Vector3.down * (characterController.height / 2f);
            //Gizmos.DrawWireSphere(checkPosition, groundCheckDistance);
            Vector3 rayPosition = bodyTransform.position;
            
            Gizmos.DrawRay(rayPosition, Vector3.down * groundCheckDistance);
        }
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
