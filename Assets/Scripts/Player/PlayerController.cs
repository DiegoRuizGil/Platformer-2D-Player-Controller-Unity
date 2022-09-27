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
    public Transform cornerDetectionLeft;
    public Transform cornerDetectionRight;
    public PhysicsMaterial2D noFrictionMaterial;
    public PhysicsMaterial2D fullFrictionMaterial;

    [Header("Movement configuration")]
    [Range(0, 100)] public float maxSpeed;
    [Range(0, 100)] public float maxAceleration;

    [Header("Jump configuration")]
    [Range(0, 20)] public float maxJumpHeight;
    public float jumpBufferTime = 0.2f;
    public float coyoteTime = 0.25f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    [Range(0, 1)] public float cornerDetectionDistance = 0.2f;
    [Range(0, 1)] public float cornerCorrectionOffset = 0.2f;
    public float groundCheckRadius;

    [Header("Slopes configuration")]
    public float slopeCheckDistance = 0.25f;

    // Movement
    private Vector2 _movement;
    private bool _grounded;
    private float _horizontalMovementTimer;

    // Jump
    private Queue<string> _jumpBuffer;
    private const string JUMP_ACTION = "JUMP";
    private float _coyoteTimer;
    private bool _higherJump;

    // Slopes
    private bool _isOnSlope;
    private Vector2 _slopeDirection;

    void Start()
    {
        _horizontalMovementTimer = 0f;
        _jumpBuffer = new Queue<string>();
        _isOnSlope = false;
    }

    void Update()
    {
        _grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (_grounded)
        {
            _coyoteTimer = coyoteTime;
        }
        else
        {
            _coyoteTimer -= Time.deltaTime;
        }

        CheckSlopes();
    }

    private void FixedUpdate()
    {
        ImproveGravity();

        Movement();

        PerformJump();

        CornerCorrection();
    }

    private void DequeueJumpAction()
    {
        if (_jumpBuffer.Count > 0)
            _jumpBuffer.Dequeue();
    }

    // ------------- MOVEMENT -------------
    public void MovementInput(InputAction.CallbackContext value)
    {
        _movement = value.ReadValue<Vector2>();
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

        if (_isOnSlope && _grounded)
        {
            xSpeed = maxSpeed * _slopeDirection.x * _movement.x;
            ySpeed = maxSpeed * _slopeDirection.y * _movement.x;
        }

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
        if (value.phase == InputActionPhase.Started) // .phase == InputActionPhase.Started
        {
            _jumpBuffer.Enqueue(JUMP_ACTION);
            Invoke(nameof(DequeueJumpAction), jumpBufferTime);
            _higherJump = true;
        }
        else if(value.phase == InputActionPhase.Canceled)
        {
            _higherJump = false;
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
    }

    private void PerformJump()
    {
        if (_jumpBuffer.Count == 0)
            return;

        if (_coyoteTimer > 0f)
        {
            Jump();
            _jumpBuffer.Dequeue();
            _coyoteTimer = 0f;
            _grounded = false;
        }
    }

    private void ImproveGravity()
    {
        // falling
        if (rb2D.velocity.y < -0.1f)
        {
            rb2D.gravityScale = fallMultiplier;
        }
        // short jump
        else if (!_higherJump)
        {
            rb2D.gravityScale = lowJumpMultiplier;
        }
        else
        {
            rb2D.gravityScale = 1f;
        }
    }

    private void CornerCorrection()
    {
        if (cornerDetectionLeft == null || cornerDetectionRight == null)
            return;

        RaycastHit2D leftRay = Physics2D.Raycast(cornerDetectionLeft.position, Vector2.up, cornerDetectionDistance, groundLayer);
        RaycastHit2D rightRay = Physics2D.Raycast(cornerDetectionRight.position, Vector2.up, cornerDetectionDistance, groundLayer);

        Debug.DrawLine(cornerDetectionLeft.position,
            cornerDetectionLeft.position + new Vector3(0f, cornerDetectionDistance, 0f),
            leftRay ? Color.red : Color.blue);
        Debug.DrawLine(cornerDetectionRight.position,
            cornerDetectionRight.position + new Vector3(0f, cornerDetectionDistance, 0f),
            rightRay ? Color.red : Color.blue);

        if (leftRay && !rightRay)
        {
            this.transform.position += new Vector3(cornerCorrectionOffset, 0f, 0f);
        }
        else if (!leftRay && rightRay)
        {
            this.transform.position -= new Vector3(cornerCorrectionOffset, 0f, 0f);
        }
    }

    // -------------- SLOPES --------------

    private void CheckSlopes()
    {
        if (_isOnSlope && _movement.x == 0f)
        {
            rb2D.sharedMaterial = fullFrictionMaterial;
        }
        else
        {
            rb2D.sharedMaterial = noFrictionMaterial;
        }

        Vector2 checkPosition = transform.position - new Vector3(0f, cc2D.size.y / 2f, 0f);

        RaycastHit2D raycastVecrtical = Physics2D.Raycast(checkPosition, Vector2.down, slopeCheckDistance, groundLayer);
        if (!raycastVecrtical)
            return;

        Vector2 perpendicular = Vector2.Perpendicular(raycastVecrtical.normal);

        Debug.DrawRay(raycastVecrtical.point, raycastVecrtical.normal, Color.red);
        Debug.DrawRay(raycastVecrtical.point, perpendicular, Color.blue);

        float slopeAngle = Vector2.Angle(Vector2.up, raycastVecrtical.normal);

        // slope going upwards from left to right
        if (perpendicular.y < 0)
        {
            _slopeDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * slopeAngle), Mathf.Sin(Mathf.Deg2Rad * slopeAngle));
        }
        // slope going upwards from right to left
        else
        {
            _slopeDirection = new Vector2(Mathf.Cos(Mathf.Deg2Rad * slopeAngle), -1f * Mathf.Sin(Mathf.Deg2Rad * slopeAngle));
        }

        _isOnSlope = slopeAngle != 0f;
    }
}
