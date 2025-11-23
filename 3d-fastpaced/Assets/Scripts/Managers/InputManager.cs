using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    [Header("Input System")]
    private PlayerInput playerInput;

    
    private InputAction moveAction;
    private InputAction lookAction;
    public InputAction sprintAction;
    public InputAction jumpAction;
    public InputAction crouchAction;
    public InputAction fireAction;
    public InputAction aimAction;
    public InputAction reloadAction;

    public bool IsMovingForward => moveInput.y > 0.1f; // w tusu

    [Header("Current Input Values")]
    public Vector2 moveInput { get; private set; }
    public Vector2 lookInput { get; private set; }
    public bool isMoving { get; private set; }
    public bool isSprinting { get; private set; }

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Get PlayerInput
        playerInput = GetComponent<PlayerInput>();
        InitializeActions();
    }

    private void InitializeActions()
    {
        // Movement
        moveAction = playerInput.actions.FindAction("Move");
        lookAction = playerInput.actions.FindAction("Look");
        sprintAction = playerInput.actions.FindAction("Sprint");
        jumpAction = playerInput.actions.FindAction("Jump");
        crouchAction = playerInput.actions.FindAction("Crouch");

        // Weapon
        fireAction = playerInput.actions.FindAction("Fire");
        aimAction = playerInput.actions.FindAction("Aim");
        reloadAction = playerInput.actions.FindAction("Reload");
    }

    private void Update()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        lookInput = lookAction.ReadValue<Vector2>();

        isMoving = moveInput.magnitude > 0.1f;
        isSprinting = sprintAction.ReadValue<float>() > 0;
    }
}

