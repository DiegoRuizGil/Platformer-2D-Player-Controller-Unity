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

    // Hurt animation
    private bool _getHurt;
    private float _hurtTimer = 0f;

    void Update()
    {
        _grounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (_getHurt)
            _hurtTimer += Time.deltaTime;
    }

    public void MovementInput(InputAction.CallbackContext value)
    {
        _movement = value.ReadValue<Vector2>();
    }

    private void LateUpdate()
    {
        // Give time to make grounded false
        if (_hurtTimer > 0.05f)
        {
            animator.SetTrigger("Hurt");
            _hurtTimer = 0f;
            _getHurt = false;
        }
        animator.SetBool("Idle", _movement.x == 0f);
        animator.SetBool("Grounded", _grounded);
        animator.SetFloat("VerticalSpeed", rb2D.velocity.y);
    }

    public void GetHurt()
    {
        _getHurt = true;
    }
}
