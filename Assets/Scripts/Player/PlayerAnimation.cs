using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Dependencies")]
    public Rigidbody2D rb2D;
    public Animator animator;
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Configuration")]
    public float groundCheckRadius;

    private bool _grounded;
    private Vector2 _movement;

    void Update()
    {
        _grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    public void MovementInput(InputAction.CallbackContext value)
    {
        _movement = value.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        animator.SetBool("Idle", _movement.x == 0f);
        animator.SetBool("Grounded", _grounded);
        animator.SetFloat("VerticalSpeed", rb2D.velocity.y);
    }
}
