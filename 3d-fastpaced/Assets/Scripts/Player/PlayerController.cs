using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


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
    private CharacterController characterController;
    [SerializeField] private Transform cameraHead;

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
    [SerializeField] private float gravity = -9.81f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;

    private Vector3 velocity;
    private bool isGrounded;
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
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        lookInput = lookAction.ReadValue<Vector2>();
        moveInput = moveAction.ReadValue<Vector2>();

        GroundCheck();
        Debug.Log(isGrounded);

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

        velocity.y += gravity * Time.deltaTime;
        Vector3 verticalMove = velocity * Time.deltaTime;

        Vector3 finalMove = horizontalMove + verticalMove;
        characterController.Move(finalMove);

    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position + Vector3.down * characterController.height/2 , 
            groundCheckDistance, 
            groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }
    private void Jump()
    {
        Debug.Log("Jumped");
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
            Gizmos.DrawWireSphere(checkPosition, groundCheckDistance);
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
