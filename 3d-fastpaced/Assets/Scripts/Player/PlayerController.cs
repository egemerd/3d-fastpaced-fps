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

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float sprintMultiplier = 2f;
    [SerializeField] private float groundStopDeceleration = 10f;

    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 2f;
    [SerializeField] private float sprintingJumpHeight = 2.5f;
    [SerializeField] private float gravity = -15f;
    [SerializeField] private float fallGravity = 1.5f;
    [SerializeField] private float jumpGravity = 1.5f;
    [SerializeField] private float airControlSpeed = -9.81f;
    [SerializeField] private float jumpRayOffset = 0.5f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private float coyoteTime = 0.15f;
    [SerializeField] private float airSpeedDecay = 3f;   // YENÝ


    [Header("Bunny Hop")]
    [SerializeField] private float bunnyHopSpeedBoost = 3f; // Hýz artýþý miktarý
    [SerializeField] private float bunnyHopBoostDuration = 0.3f; // Boost süresi
    [SerializeField] private float bunnyHopWindow = 0.2f;
    private float lastLandTime;
    private bool canBunnyHop;
    private Coroutine bunnyHopCoroutine;
    public bool bunnyHopActivated;

    [Header("Sliding")]
    [SerializeField] private float slideSpeed = 15f;
    [SerializeField] private float slideSpeedDecay = 5f;
    [SerializeField] private float minSlideInitialSpeed = 1f; // min to start sliding
    [SerializeField] private float slideCooldown = 0.1f;
    [SerializeField] private float slideControlMultiplier = 0.5f;
    [SerializeField] private float crouchHeight = 1f;
    [SerializeField] private float standingHeight = 2f;
    [SerializeField] private Vector3 slidingRotation;

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
    private bool wasGrounded;

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

        CheckBunnyHopWindow();

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

        currentState.UpdateState(this,climbing);
        //cameraShake.SprintSway(moveInput); 
        //cameraShake.SprintFovBoost(isSprinting,isSliding,moveInput);
        //cameraShake.SprintFovBoostWeapon(isSprinting,isSliding,moveInput);
        stateText.text = currentState.StateName;
        if (speedText != null)
        {
            float currentSpeed = horizontalVelocity.magnitude;
            
            speedText.text = $"SPEED: {currentSpeed:F1} m/s";
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
        Vector3 desiredVelocity = CalculateDesiredVelocity();

        if (isSliding)
        {
            ApplySlideMovement();
            return;
        }

        if (isGrounded)
        {
            horizontalVelocity = CalculateGroundedVelocity(desiredVelocity);
        }
        else
        {
            horizontalVelocity = CalculateAirVelocity(desiredVelocity);
        }

        ApplyFinalMovement();
    }

    private Vector3 CalculateDesiredVelocity()
    {
        float speedMultiplier = isSprinting ? sprintMultiplier : 1f;
        return moveDirection * moveSpeed * speedMultiplier;
    }

    private void ApplySlideMovement()
    {
        Vector3 verticalMove = velocity * Time.deltaTime;
        Vector3 horizontalMove = horizontalVelocity * Time.deltaTime;
        characterController.Move(horizontalMove + verticalMove);
    }

    private Vector3 CalculateGroundedVelocity(Vector3 desiredVelocity)
    {
        // Yerdeyken hýz fazlasý HER ZAMAN anlýk kesilir (momentum korunmaz).
        // Slide-jump sonrasý yere inince "sarhoþluk" hissi yaratmamak için bilinçli tercih.

        if (!isMoving)
        {
            return Vector3.Lerp(horizontalVelocity, Vector3.zero, groundStopDeceleration * Time.deltaTime);
        }

        // Sprint býrakma dahil her durumda hedefe anlýk geç.
        return desiredVelocity;
    }

    private Vector3 CalculateAirVelocity(Vector3 desiredVelocity)
    {
        float currentSpeed = horizontalVelocity.magnitude;
        float desiredSpeed = desiredVelocity.magnitude;

        if (currentSpeed > desiredSpeed)
        {
            // Yön input'a doðru yumuþakça çevrilir
            Vector3 currentDirection = horizontalVelocity.normalized;
            Vector3 targetDirection = Vector3.Lerp(currentDirection, moveDirection, airControlSpeed * Time.deltaTime);

            // Hýz yavaþça normale doðru azalýr (anlýk kesilmez, boost hissi korunur ama sonsuza kadar sürmez)
            float newSpeed = currentSpeed - (airSpeedDecay * Time.deltaTime);
            newSpeed = Mathf.Max(newSpeed, desiredSpeed);

            return targetDirection.normalized * newSpeed;
        }

        return Vector3.Lerp(horizontalVelocity, desiredVelocity, airControlSpeed * Time.deltaTime);
    }

    private void ApplyFinalMovement()
    {
        Vector3 verticalMove = velocity * Time.deltaTime;
        Vector3 horizontalMove = horizontalVelocity * Time.deltaTime;
        characterController.Move(horizontalMove + verticalMove);
    }

    private void GroundCheck()
    {
        Vector3 bodyPosition = bodyTransform.position;
        bool physicsGrounded = Physics.Raycast(bodyPosition , 
            Vector3.down , 
            (characterController.height/2 + jumpRayOffset),
            groundMask);
        bool isAscending = velocity.y > 0.1f;
        if (isAscending)
        {
            physicsGrounded = false;
        }

        if (physicsGrounded && !wasGrounded)
        {
            OnLanded();
        }
        if (physicsGrounded)
        {
            lastGroundedTime = Time.time;
            isGrounded = true;
        }
        else if (isAscending)
        {
            // Yükseliyorsak coyote time'ý hiç sorgulama, kesin olarak havadayýz.
            isGrounded = false;
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
    private void OnLanded()
    {
        lastLandTime = Time.time;
        canBunnyHop = true;
    }

    private void CheckBunnyHopWindow()
    {
        if (canBunnyHop && Time.time - lastLandTime > bunnyHopWindow)
        {
            canBunnyHop = false;
        }
    }

    private void ActivateBunnyHop()
    {
        if (bunnyHopCoroutine != null)
        {
            StopCoroutine(bunnyHopCoroutine);
        }

        bunnyHopCoroutine = StartCoroutine(BunnyHopBoost());
    }



    private IEnumerator BunnyHopBoost()
    {
        float baseSpeed = moveSpeed;
        moveSpeed += bunnyHopSpeedBoost;

        yield return new WaitForSeconds(bunnyHopBoostDuration);

        moveSpeed = baseSpeed;
        bunnyHopCoroutine = null;
    }
    public void ToggleBunny()
    {
        bunnyHopActivated = !bunnyHopActivated;
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

            //Debug.Log($"Slide Jump: Speed = {totalForwardSpeed}, Direction = {slideDirection}");
            //Debug.Log($"Horizontal Velocity after Slide Jump: {horizontalVelocity.magnitude}");
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
        if (canBunnyHop && !isSliding && bunnyHopActivated)
        {
            ActivateBunnyHop();
            canBunnyHop = false; // Bir kere kullanýlsýn
        }
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
        if (InputManager.Instance.IsInputLocked) return;
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

        if (!InputManager.Instance.IsInputLocked) // YENÝ: sadece kilitli DEÐÝLKEN input kontrolü yap
        {
            bool crouchReleased = input.crouchAction.ReadValue<float>() == 0f;
            bool sprintReleased = !isSprinting;

            if (crouchReleased || sprintReleased)
            {
                slideEnded = true;
                return;
            }
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
        slideEnded = false;
        isSliding = true;
        slideTimer = 0f;

        slideDirection = horizontalVelocity.normalized;

        currentSlideSpeed = Mathf.Max(horizontalVelocity.magnitude, slideSpeed);

        //cameraHead.localPosition = new Vector3(0, -0.150000006f, -0.0160000008f);
        characterController.height = crouchHeight;
        targetCameraHeight = originalCameraHeight - (standingHeight - crouchHeight);
        bodyTransform.localRotation = Quaternion.Euler(slidingRotation.x, 0f, 0f);



    }

    public void StopSlide()
    {
        isSliding = false;

        characterController.height = standingHeight;
        targetCameraHeight = originalCameraHeight;

        bodyTransform.localRotation = Quaternion.Euler(0f, bodyTransform.localRotation.y, bodyTransform.localRotation.z);

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
