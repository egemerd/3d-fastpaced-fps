using System;
using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;


public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraHead;
    [SerializeField] private Transform bodyTransform;
    private InputManager input;
    CameraShakeMovement cameraShake;
    Climbing climbing;

    private Camera mainCamera;
    private CharacterController characterController;
    

    [Header("Input")]
    private Vector2 moveInput;
    private Vector2 lookInput;
    private float verticalRotation;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float airMovementController = 0.8f;


    [Header("Look")]
    [SerializeField] private float mouseSens;
    [SerializeField] private float lookRange;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float sprintingJumpHeight = 2.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float fallGravity = 1.5f;
    [SerializeField] private float jumpGravity = 1.5f;
    [SerializeField] private float airControlTime = -9.81f;
    [SerializeField] private float jumpRayOffset = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float coyoteTime = 0.15f;

    [Header("Sliding")]
    [SerializeField] private float slideSpeed = 15f;
    [SerializeField] private float slideSpeedDecay = 5f;
    [SerializeField] private float minSlideSpeed = 1f; // min to stay sliding
    [SerializeField] private float minSlideInitialSpeed = 1f; // min to start sliding
    [SerializeField] private float slideDuration = 2f;
    [SerializeField] private float slideCooldown = 0.1f;
    [SerializeField] private float slideControlMultiplier = 0.5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;

    [Header("Slide Jump Boost")]
    [SerializeField] private float slideJumpHeightMultiplier = 1.5f; // Jump boost (1.5 = 50% higher)
    [SerializeField] private float slideJumpForwardBoost = 8f; // Forward speed boost
    [SerializeField] private bool maintainSlideSpeedOnJump = true; // Keep momentum

    [Header("Momentum Settings")]
    [SerializeField] private float momentumDecayRate = 5f;

    //Movement
    private Vector3 velocity;

    [Header("Checkers")]
    public bool isGrounded;
    public bool isMoving;  
    Vector3 moveDirection;
    public bool isSprinting;
    private Vector3 horizontalVelocity;

    //Sliding
    public bool isSliding;
    private float slideTimer;
    public bool canSlide = true;
    private Vector3 slideDirection;
    private float currentSlideSpeed;
    public bool slideEnded;

    //Jumping
    private float lastGroundedTime;

    //camera height
    private float originalCameraHeight;
    private float targetCameraHeight;
    [SerializeField] private float cameraTransitionSpeed = 10f;
    private Transform originalHeadTransform;

    private IState currentState;

    [SerializeField] private TextMeshProUGUI stateText;
    [SerializeField] private TextMeshProUGUI speedText;


    private void Start()
    {
        input = InputManager.Instance;

        mainCamera = Camera.main;

        cameraShake = GetComponent<CameraShakeMovement>();
        climbing = GetComponent<Climbing>();    

        

        originalCameraHeight = cameraHead.localPosition.y;
        targetCameraHeight = originalCameraHeight;

        characterController.height = standingHeight;
        originalHeadTransform = cameraHead;
        currentState = new IdleState(); 
        currentState.EnterState(this);  
    }
    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        
    }
    private void Update()
    {
        lookInput = input.lookInput;
        moveInput =input.moveInput;

        isMoving = input.isMoving;
        isSprinting = input.isSprinting;


        GroundCheck();
        //Debug.Log(isMoving);
        if (climbing == null || !climbing.isClimbing)
        {
            ApplyGravity();
        }
        else
        {
            velocity.y = 0f;
        }
        SmoothCameraHeight();
        Debug.Log("[PlayerController] isSliding" + isSliding);
        currentState.UpdateState(this,climbing);
        //cameraShake.SprintSway(moveInput); 
        //cameraShake.SprintFovBoost(isSprinting,isSliding,moveInput);
        //cameraShake.SprintFovBoostWeapon(isSprinting,isSliding,moveInput);
        stateText.text = currentState.StateName;
        if (speedText != null)
        {
            float currentSpeed = horizontalVelocity.magnitude;
            
            speedText.text = $"Speed: {currentSpeed:F1} m/s";
        }

        
        //if (jumpAction.triggered && isGrounded)
        //{
        //    Jump();
        //}

        //CalculateMoveDirection();
        //Movement();
    }

    public InputAction GetJumpAction() => input.jumpAction;
    public InputAction GetCrouchAction() => input.crouchAction;

    public void ChangeState(IState newState)
    {
        currentState.ExitState(this);
        currentState = newState;
        currentState.EnterState(this);
    }

    public void ResetVerticalVelocity()
    {
        velocity.y = 0f;
    }
    public void Movement()
    {
        float speedMultiplier = isSprinting  ? sprintMultiplier : 1f;
        Vector3 desiredVelocity = moveDirection * moveSpeed * speedMultiplier;

        if (isSliding)
        {
            Vector3 verticalMove2 = velocity * Time.deltaTime;
            Vector3 horizontalMove2 = horizontalVelocity * Time.deltaTime;
            Vector3 finalMove2 = horizontalMove2 + verticalMove2;
            characterController.Move(finalMove2);
            return; 
        }
        if (isGrounded)
        {
            float currentSpeed = horizontalVelocity.magnitude;
            float desiredSpeed = desiredVelocity.magnitude;
            if (!isMoving)
            {
                //hýzlý dur
                horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero,
                    10f * Time.deltaTime); 
            }
            else if (!isSprinting && currentSpeed > desiredSpeed && currentSpeed < (moveSpeed * sprintMultiplier * 1.1f))
            {
                horizontalVelocity = desiredVelocity;
            }
            else if (currentSpeed>desiredSpeed)
            {
                Vector3 currentDirection = horizontalVelocity.normalized;

                float newSpeed = currentSpeed - (slideSpeedDecay * Time.deltaTime);
                newSpeed = Mathf.Max(newSpeed, desiredSpeed);

                Vector3 targetDirection = Vector3.Lerp(currentDirection, moveDirection, 
                    momentumDecayRate * Time.deltaTime);

                horizontalVelocity = targetDirection.normalized * newSpeed;
            
            }
            else
            {
                horizontalVelocity = desiredVelocity;

            }

        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, desiredVelocity, 
                airControlTime * Time.deltaTime);
        }

        Vector3 verticalMove = velocity * Time.deltaTime;
        Vector3 horizontalMove = horizontalVelocity * Time.deltaTime;   
        Vector3 finalMove = horizontalMove + verticalMove;
        characterController.Move(finalMove);

    }

    private void GroundCheck()
    {
        Vector3 bodyPosition = bodyTransform.position;
        bool physicsGrounded = Physics.Raycast(bodyPosition , 
            Vector3.down , 
            (characterController.height/2 + jumpRayOffset),
            groundMask);
        if(physicsGrounded)
        {
            lastGroundedTime = Time.time;
            isGrounded = true;
        }
        else
        {
            float timeSinceGrounded = Time.time - lastGroundedTime;

            isGrounded = timeSinceGrounded <= coyoteTime;
        }
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }

    
    private void NormalJump()
    {
       
        float height = jumpHeight;

        if(isSprinting)
        {
            height = sprintingJumpHeight;
            
        }
        velocity.y = Mathf.Sqrt(height * -2 * gravity);
    }

    private void SlideJumpBoost()
    {
        float boostedHeight = jumpHeight * slideJumpHeightMultiplier;
        velocity.y = Mathf.Sqrt(boostedHeight * -2 * gravity);

        if(maintainSlideSpeedOnJump)
        {
            float totalForwardSpeed = currentSlideSpeed + slideJumpForwardBoost;
            horizontalVelocity = slideDirection * totalForwardSpeed;

            Debug.Log($"Slide Jump: Speed = {totalForwardSpeed}, Direction = {slideDirection}");
            Debug.Log($"Horizontal Velocity after Slide Jump: {horizontalVelocity.magnitude}");
        }
        else
        {
            horizontalVelocity += slideDirection * slideJumpForwardBoost;
        }

        StopSlide();
    }

    public void Jump()
    {
        AudioManager.Instance.PlaySFX("PlayerJump" , 0.4f);
        if (isSliding)
        {
            SlideJumpBoost();
        }
        else
        {
            NormalJump();
        }

    }

    private void ApplyGravity()
    {
        float currentGravity = gravity;

        if (velocity.y < 0)
        {
            //Debug.Log("Applying Fall Gravity");
            //Debug.Log(velocity.y);
            currentGravity *= fallGravity;
        }
        else if (velocity.y > 0)
        {
            currentGravity *= jumpGravity;
        }
        //Debug.Log("Current Gravity: " + currentGravity);
        velocity.y += currentGravity * Time.deltaTime;
    }

    public void CalculateMoveDirection()
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
        //Debug.Log("New Speed: " + moveSpeed);
    }
    private IEnumerator SpeedBoost(float duration, float amount)
    {
        float baseSpeed = moveSpeed;
        moveSpeed += amount;
        yield return new WaitForSeconds(duration);
        moveSpeed = baseSpeed;
    }
    
    public float GetVelocityY()
    {
        return velocity.y;
    }

    public float GetVelocity()
    {
        return horizontalVelocity.magnitude;
    }

    public bool MinSlideSpeedReached()
    {
        return horizontalVelocity.magnitude >= minSlideInitialSpeed;
    }


    public void HandleSliding()
    {
        if (!isSliding) return; 

        bool crouchReleased = input.crouchAction.ReadValue<float>() == 0f;
        bool sprintReleased = !isSprinting;

        if (crouchReleased || sprintReleased)
        {
            slideEnded = true;
            return; 
        }

        slideTimer += Time.deltaTime;

        currentSlideSpeed -= slideSpeedDecay * Time.deltaTime;
        currentSlideSpeed = Mathf.Max(currentSlideSpeed, 0f);

        horizontalVelocity = slideDirection * currentSlideSpeed;

        if (isMoving)
        {
            CalculateMoveDirection();
            slideDirection = Vector3.Lerp(slideDirection, moveDirection,
                slideControlMultiplier * Time.deltaTime).normalized;
        }
    }

    public void StartSlide()
    {
        Debug.Log("Starting Slide");
        slideEnded = false;
        isSliding = true;
        slideTimer = 0f;

        slideDirection = horizontalVelocity.normalized;

        currentSlideSpeed = Mathf.Max(horizontalVelocity.magnitude, slideSpeed);

        //cameraHead.localPosition = new Vector3(0, -0.150000006f, -0.0160000008f);
        characterController.height = crouchHeight;
        targetCameraHeight = originalCameraHeight - (standingHeight - crouchHeight);

        
    }

    public void StopSlide()
    {
        Debug.Log("Stopping Slide");
        isSliding = false;

        characterController.height = standingHeight;
        targetCameraHeight = originalCameraHeight;
        //cameraHead.localPosition = originalHeadTransform.localPosition;
        StartCoroutine(SlideCooldown());
        
    }


    private IEnumerator SlideCooldown()
    {
        canSlide = false;
        yield return new WaitForSeconds(slideCooldown);
        canSlide = true;
    }

    private void SmoothCameraHeight()
    {
        Vector3 currentPos = cameraHead.localPosition;
        currentPos.y = Mathf.Lerp(currentPos.y, targetCameraHeight,
            cameraTransitionSpeed * Time.deltaTime);
        cameraHead.localPosition = currentPos;
    }
}
