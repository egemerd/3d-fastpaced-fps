using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    Rigidbody rb;
    private InputAction moveAction;
    private PlayerInput playerInput;
    
    [Header("Input")]
    private Vector2 moveInput;

    [Header("Movement")]
    [SerializeField] float moveSpeed;

    private void Start()
    {
        moveAction = playerInput.actions.FindAction("Move");
    }
    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        Debug.Log(moveInput);
        Vector3 direction = new Vector3(moveInput.x , 0 , moveInput.y);
        direction.y = 0;
        rb.linearVelocity = direction * Time.deltaTime * moveSpeed;
    }
}
