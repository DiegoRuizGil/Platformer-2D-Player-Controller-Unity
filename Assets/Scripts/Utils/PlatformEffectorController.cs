using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlatformEffectorController : MonoBehaviour
{
    private PlatformEffector2D _pe2D;
    private float _verticalInput;
    private bool _hasLeftPlaform;
    private bool _isPlayerOnTop;


    private void Start()
    {
        _pe2D = GetComponent<PlatformEffector2D>();
    }

    private void Update()
    {
        if (_pe2D == null)
            return;

        if (_verticalInput < 0 && !_hasLeftPlaform && _isPlayerOnTop)
        {
            _pe2D.rotationalOffset = 180;
            _hasLeftPlaform = true;
        }
    }

    public void MovementInput(InputAction.CallbackContext value)
    {
        _verticalInput = value.ReadValue<Vector2>().y;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        _isPlayerOnTop = collision.transform.tag == "Player";
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (_pe2D == null)
            return;

        _pe2D.rotationalOffset = 0;
        _hasLeftPlaform = false;
        _isPlayerOnTop = false;
    }
}
