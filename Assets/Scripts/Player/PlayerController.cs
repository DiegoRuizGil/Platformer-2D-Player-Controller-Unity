using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody2D rb2D;
    public CapsuleCollider2D cc2D;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Movement configuration")]
    [Range(0, 100)] public float maxSpeed;
    [Range(0, 100)] public float maxAceleration;

    [Header("Jump configuration")]
    [Range(0, 20)] public float maxJumpHeight;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.25f;
    public float groundCheckRadius;

    // Movement
    private Vector2 _movement;
    private bool _grounded;
    private float _horizontalMovementTimer;

    // Jump
    private Queue<string> _jumpBuffer;
    private const string JUMP_ACTION = "JUMP";

    void Start()
    {
        _horizontalMovementTimer = 0f;

        _jumpBuffer = new Queue<string>();
    }

    void Update()
    {
        _grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void FixedUpdate()
    {
        Movement();

        if (_grounded && _jumpBuffer.Count > 0)
        {
            Jump();
            CancelInvoke(nameof(DequeueJumpAction));
            _jumpBuffer.Dequeue();
        }
    }

    private void DequeueJumpAction()
    {
        if (_jumpBuffer.Count > 0)
            _jumpBuffer.Dequeue();
    }

    // ------------- MOVEMENT -------------
    public void MovementInput(InputAction.CallbackContext value)
    {
        _movement = new Vector2(value.ReadValue<Vector2>().x, value.ReadValue<Vector2>().y);
        _horizontalMovementTimer = 0f;
    }

    private void Movement()
    {
        if (_movement == Vector2.zero && _grounded)
        {
            rb2D.velocity = Vector2.zero;
            return;
        }

        float xSpeed = GetHorizontalSpeed();
        float ySpeed = _grounded ? 0f : rb2D.velocity.y;

        rb2D.velocity = new Vector2(xSpeed, ySpeed);
    }

    private float GetHorizontalSpeed()
    {
        float horizontalSpeed;

        if (Mathf.Abs(rb2D.velocity.x) >= maxSpeed)
        {
            horizontalSpeed = _movement.x * maxSpeed;
        }
        else
        {
            // v = a*t + v0
            _horizontalMovementTimer += Time.deltaTime;
            horizontalSpeed = _movement.x * maxAceleration * _horizontalMovementTimer + rb2D.velocity.x;
        }

        return horizontalSpeed;
    }

    // --------------- JUMP ---------------

    public void JumpInput(InputAction.CallbackContext value)
    {
        if (value.phase == InputActionPhase.Started)
        {
            _jumpBuffer.Enqueue(JUMP_ACTION);
            Invoke(nameof(DequeueJumpAction), jumpBufferTime);
        }
    }

    private void Jump()
    {
        // g = -2*h/t_h^2
        // v = -g*t_h

        float gravity = Physics2D.gravity.y;
        float maxHeightTime = Mathf.Sqrt(-2f * maxJumpHeight / gravity);
        float verticalSpeed = -1f * gravity * maxHeightTime;

        rb2D.velocity = new Vector2(rb2D.velocity.x, verticalSpeed);
        _grounded = false;
    }
}
