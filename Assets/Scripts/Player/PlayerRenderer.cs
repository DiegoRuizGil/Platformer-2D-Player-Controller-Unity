using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRenderer : MonoBehaviour
{
    [Header("Dependencies")]
    public SpriteRenderer spriteRenderer;

    public void MovementInput(InputAction.CallbackContext value)
    {
        Vector2 movement = value.ReadValue<Vector2>();

        if(movement.x > 0f && LookingLeft())
        {
            spriteRenderer.flipX = false;
        }
        else if(movement.x < 0f && !LookingLeft())
        {
            spriteRenderer.flipX = true;
        }
    }

    private bool LookingLeft()
    {
        return spriteRenderer.flipX;
    }
}
